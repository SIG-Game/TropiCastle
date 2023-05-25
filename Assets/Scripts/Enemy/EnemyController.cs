using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int playerDamageAmount;
    [SerializeField] private float playerKnockbackForce;
    [SerializeField] private float maxStartChasingDistanceToPlayer;
    [SerializeField] private float minStopChasingDistanceToPlayer;
    [SerializeField] private float initialWaitTimeBeforeChilling;
    [SerializeField] private float waitTimeAfterKnockback;
    [SerializeField] private float fadeOutSpeed;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private List<ItemWithAmount> droppedLoot;

    private Rigidbody2D rb2d;
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private HealthController healthController;
    private Vector2 playerColliderOffset;
    private EnemyState state;

    public float LastHitTime { get; set; }

    private void Awake()
    {
        LastHitTime = 0f;

        rb2d = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthController = GetComponent<HealthController>();

        state = EnemyState.Initial;

        StartCoroutine(InitialWaitBeforeChillingCoroutine());

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;
    }

    private void Start()
    {
        // Not in Awake because this needs to happen after EnemySpawner sets playerTransform
        playerColliderOffset = playerTransform.GetComponent<BoxCollider2D>().offset;
    }

    private void Update()
    {
        if (state == EnemyState.Chilling)
        {
            StartChasingPlayerIfPlayerInRange();
        }

        if (state == EnemyState.Chasing)
        {
            StopChasingPlayerIfPlayerOutOfRange();
        }

        if (state == EnemyState.FadingOut)
        {
            FadeOut();
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

    private void StartChasingPlayerIfPlayerInRange()
    {
        if (GetDistanceToPlayerCollider() <= maxStartChasingDistanceToPlayer)
        {
            state = EnemyState.Chasing;
        }
    }

    private void StopChasingPlayerIfPlayerOutOfRange()
    {
        if (GetDistanceToPlayerCollider() >= minStopChasingDistanceToPlayer)
        {
            state = EnemyState.Chilling;
        }
    }

    private void FadeOut()
    {
        float newAlpha = spriteRenderer.color.a - fadeOutSpeed * Time.deltaTime;

        if (newAlpha <= 0f)
        {
            newAlpha = 0f;

            EnemyDeath();
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);

        if (healthText != null)
        {
            healthText.color = new Color(healthText.color.r,
                healthText.color.g, healthText.color.b, newAlpha);
        }
    }

    private void EnemyDeath()
    {
        foreach (ItemWithAmount loot in droppedLoot)
        {
            if (!playerInventory.CanAddItem(loot))
            {
                InventoryFullUIController.Instance.ShowInventoryFullText();

                ItemWorldPrefabInstanceFactory.Instance.DropItem(transform.position, loot);
            }
            else
            {
                playerInventory.AddItem(loot);
            }
        }

        Destroy(gameObject);
    }

    private void HealthController_OnHealthSetToZero()
    {
        state = EnemyState.FadingOut;

        collider2D.enabled = false;
    }

    //gets knockback when in contact with player
    //can update to when getting hit by weapon by changing the tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthController playerHealthController = other.GetComponent<HealthController>();
            if (playerHealthController != null)
            {
                playerHealthController.DecreaseHealth(playerDamageAmount);
            }

            if (state != EnemyState.KnockedBack)
            {
                Vector2 directionFromPlayer = ((Vector2)transform.position - GetPlayerColliderPosition()).normalized;
                ApplyKnockback(directionFromPlayer, playerKnockbackForce);
            }
        }
    }

    public void ApplyKnockback(Vector2 normalizedDirection, float force)
    {
        if (state == EnemyState.KnockedBack)
        {
            rb2d.velocity = Vector2.zero;
        }

        if (state != EnemyState.FadingOut)
        {
            state = EnemyState.KnockedBack;
        }

        rb2d.AddForce(normalizedDirection * force, ForceMode2D.Impulse);

        StopCoroutine(nameof(WaitAfterKnockbackCoroutine));
        StartCoroutine(nameof(WaitAfterKnockbackCoroutine));
    }

    private IEnumerator InitialWaitBeforeChillingCoroutine()
    {
        yield return new WaitForSeconds(initialWaitTimeBeforeChilling);
        state = EnemyState.Chilling;
    }

    private IEnumerator WaitAfterKnockbackCoroutine()
    {
        yield return new WaitForSeconds(waitTimeAfterKnockback);

        if (state != EnemyState.FadingOut)
        {
            state = EnemyState.Chasing;
        }

        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private Vector2 GetPlayerColliderPosition() =>
        (Vector2)playerTransform.position + playerColliderOffset;

    private float GetDistanceToPlayerCollider() =>
        Vector2.Distance(transform.position, GetPlayerColliderPosition());

    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    public void SetPlayerInventory(Inventory playerInventory)
    {
        this.playerInventory = playerInventory;
    }
}
