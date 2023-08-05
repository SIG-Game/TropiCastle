using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour,
    ISavable<EnemyController.SerializableEnemyState>
{
    [SerializeField] private float speed;
    [SerializeField] private int playerDamageAmount;
    [SerializeField] private float playerKnockbackForce;
    [SerializeField] private float maxStartChasingDistanceToPlayer;
    [SerializeField] private float minStopChasingDistanceToPlayer;
    [SerializeField] private float initialWaitTimeBeforeIdle;
    [SerializeField] private float waitTimeAfterKnockback;
    [SerializeField] private float fadeOutSpeed;
    [SerializeField] private CanvasGroup healthBarCanvasGroup;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private List<ItemWithAmount> droppedLoot;

    private Rigidbody2D rb2d;
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private HealthController healthController;
    private Spawnable spawnable;
    private Coroutine initialWaitTimeBeforeIdleCoroutine;
    private WaitForSeconds beforeIdleWaitForSecondsObject;
    private WaitForSeconds afterKnockbackWaitForSecondsObject;
    private Vector2 playerColliderOffset;
    private EnemyState state;
    private bool initialWaitOccurring;

    public float LastHitTime { get; set; }

    private void Awake()
    {
        LastHitTime = 0f;

        rb2d = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthController = GetComponent<HealthController>();
        spawnable = GetComponent<Spawnable>();

        state = EnemyState.Initial;

        beforeIdleWaitForSecondsObject =
            new WaitForSeconds(initialWaitTimeBeforeIdle);
        afterKnockbackWaitForSecondsObject =
            new WaitForSeconds(waitTimeAfterKnockback);

        initialWaitTimeBeforeIdleCoroutine =
            StartCoroutine(InitialWaitBeforeIdleCoroutine());

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;
    }

    private void Update()
    {
        if (state == EnemyState.Idle)
        {
            IdleStateUpdate();
        }
        else if (state == EnemyState.Chasing)
        {
            ChasingStateUpdate();
        }
        else if (state == EnemyState.FadingOut)
        {
            FadingOutStateUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Chasing)
        {
            rb2d.MovePosition(Vector2.MoveTowards(transform.position,
                GetPlayerColliderPosition(), speed));
        }
    }

    private void OnDestroy()
    {
        if (healthController != null)
        {
            healthController.OnHealthSetToZero -= HealthController_OnHealthSetToZero;
        }
    }

    private void IdleStateUpdate()
    {
        if (GetDistanceToPlayerCollider() <= maxStartChasingDistanceToPlayer)
        {
            state = EnemyState.Chasing;
        }
    }

    private void ChasingStateUpdate()
    {
        if (GetDistanceToPlayerCollider() >= minStopChasingDistanceToPlayer)
        {
            state = EnemyState.Idle;
        }
    }

    private void FadingOutStateUpdate()
    {
        float newAlpha = spriteRenderer.color.a - fadeOutSpeed * Time.deltaTime;

        if (newAlpha <= 0f)
        {
            newAlpha = 0f;

            EnemyDeath();
        }

        SetAlpha(newAlpha);
    }

    private void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if (healthBarCanvasGroup != null)
        {
            healthBarCanvasGroup.alpha = alpha;
        }
    }

    private void EnemyDeath()
    {
        foreach (ItemWithAmount loot in droppedLoot)
        {
            playerInventory.TryAddItem(loot, out int amountAdded);

            bool lootItemNotFullyAdded = amountAdded != loot.amount;
            if (lootItemNotFullyAdded)
            {
                ItemWithAmount itemToDrop = new ItemWithAmount(loot.itemData,
                    loot.amount - amountAdded, loot.instanceProperties);

                ItemWorldPrefabInstanceFactory.Instance.DropItem(
                    transform.position, itemToDrop);

                InventoryFullUIController.Instance.ShowInventoryFullText();
            }
        }

        Destroy(gameObject);
    }

    private void HealthController_OnHealthSetToZero()
    {
        state = EnemyState.FadingOut;

        collider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthController playerHealthController = other.GetComponent<HealthController>();
            playerHealthController.DecreaseHealth(playerDamageAmount);

            if (state != EnemyState.KnockedBack)
            {
                Vector2 directionFromPlayerToEnemy = GetDirectionFromPlayerToEnemy();
                ApplyKnockback(directionFromPlayerToEnemy, playerKnockbackForce);
            }
        }
    }

    public void ApplyKnockback(Vector2 normalizedDirection, float force)
    {
        if (initialWaitOccurring)
        {
            StopCoroutine(initialWaitTimeBeforeIdleCoroutine);
        }

        if (state == EnemyState.KnockedBack)
        {
            rb2d.velocity = Vector2.zero;
        }
        else if (state != EnemyState.FadingOut)
        {
            state = EnemyState.KnockedBack;
        }

        rb2d.AddForce(normalizedDirection * force, ForceMode2D.Impulse);

        StopCoroutine(nameof(WaitAfterKnockbackCoroutine));
        StartCoroutine(nameof(WaitAfterKnockbackCoroutine));
    }

    private IEnumerator InitialWaitBeforeIdleCoroutine()
    {
        initialWaitOccurring = true;

        yield return beforeIdleWaitForSecondsObject;
        state = EnemyState.Idle;

        initialWaitOccurring = false;
    }

    private IEnumerator WaitAfterKnockbackCoroutine()
    {
        yield return afterKnockbackWaitForSecondsObject;

        if (state != EnemyState.FadingOut)
        {
            state = EnemyState.Chasing;
        }

        rb2d.velocity = Vector2.zero;
    }

    private Vector2 GetPlayerColliderPosition() =>
        (Vector2)playerTransform.position + playerColliderOffset;

    private Vector2 GetDirectionFromPlayerToEnemy() =>
        ((Vector2)transform.position - GetPlayerColliderPosition()).normalized;

    private float GetDistanceToPlayerCollider() =>
        Vector2.Distance(transform.position, GetPlayerColliderPosition());

    public void SetUpEnemy(Transform playerTransform, Inventory playerInventory)
    {
        this.playerTransform = playerTransform;
        this.playerInventory = playerInventory;

        playerColliderOffset = playerTransform.GetComponent<BoxCollider2D>().offset;
    }

    public SerializableEnemyState GetSerializableState()
    {
        var serializableState = new SerializableEnemyState
        {
            Position = transform.position,
            Health = healthController.CurrentHealth,
            SpawnerId = spawnable.GetSpawnerId()
        };

        return serializableState;
    }

    public void SetPropertiesFromSerializableState(
        SerializableEnemyState serializableState)
    {
        transform.position = serializableState.Position;

        healthController.CurrentHealth = serializableState.Health;

        GetComponent<Spawnable>()
            .SetSpawnerUsingId<EnemySpawner>(serializableState.SpawnerId);
    }

    [Serializable]
    public class SerializableEnemyState
    {
        public Vector2 Position;
        public int Health;
        public int SpawnerId;
    }
}
