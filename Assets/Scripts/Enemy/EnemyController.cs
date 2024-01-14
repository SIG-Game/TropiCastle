using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int playerDamageAmount;
    [SerializeField] private float playerKnockbackForce;
    [SerializeField] private float maxStartChasingDistanceToPlayer;
    [SerializeField] private float minStopChasingDistanceToPlayer;
    [SerializeField] private float initialWaitTimeBeforeIdleSeconds;
    [SerializeField] private float waitTimeAfterKnockbackSeconds;
    [SerializeField] private float fadeOutSpeed;
    [SerializeField] private SpriteRenderer healthBarBackground;
    [SerializeField] private SpriteRenderer healthBarFill;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private List<ItemStack> loot;

    [Inject] private ItemWorldPrefabInstanceFactory itemWorldPrefabInstanceFactory;
    [Inject("PlayerTransform")] private Transform playerTransform;

    public InitialEnemyState InitialState { get; private set; }
    public IdleEnemyState IdleState { get; private set; }
    public ChasingEnemyState ChasingState { get; private set; }
    public KnockedBackEnemyState KnockedBackState { get; private set; }
    public FadingOutEnemyState FadingOutState { get; private set; }

    public float LastHitTime { get; set; }

    public float InitialWaitTimeBeforeIdleSeconds => initialWaitTimeBeforeIdleSeconds;
    public float WaitTimeAfterKnockbackSeconds => waitTimeAfterKnockbackSeconds;

    private BaseEnemyState currentState;
    private new Rigidbody2D rigidbody2D;
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private HealthController healthController;
    private Vector2 playerColliderOffset;

    private void Awake()
    {
        this.InjectDependencies();

        InitialState = new InitialEnemyState(this);
        IdleState = new IdleEnemyState(this);
        ChasingState = new ChasingEnemyState(this);
        KnockedBackState = new KnockedBackEnemyState(this);
        FadingOutState = new FadingOutEnemyState(this);

        LastHitTime = 0f;

        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthController = GetComponent<HealthController>();

        playerColliderOffset = playerTransform.GetComponent<BoxCollider2D>().offset;

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;

        currentState = InitialState;

        currentState.StateEnter();
    }

    private void Update()
    {
        currentState.StateUpdate();
    }

    private void FixedUpdate()
    {
        if (currentState == ChasingState)
        {
            rigidbody2D.MovePosition(Vector2.MoveTowards(transform.position,
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

    public void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if (healthBarBackground != null)
        {
            healthBarBackground.color = new Color(healthBarBackground.color.r,
                healthBarBackground.color.g, healthBarBackground.color.b, alpha);
        }

        if (healthBarFill != null)
        {
            healthBarFill.color = new Color(healthBarFill.color.r,
                healthBarFill.color.g, healthBarFill.color.b, alpha);
        }
    }

    public void EnemyDeath()
    {
        foreach (ItemStack lootItem in loot)
        {
            playerInventory.TryAddItem(lootItem, out int amountAdded);

            bool lootItemNotFullyAdded = amountAdded != lootItem.Amount;
            if (lootItemNotFullyAdded)
            {
                ItemStack itemToDrop = lootItem
                    .GetCopyWithAmount(lootItem.Amount - amountAdded); 

                itemWorldPrefabInstanceFactory.DropItem(
                    transform.position, itemToDrop);
            }
        }

        Destroy(gameObject);
    }

    private void HealthController_OnHealthSetToZero()
    {
        SwitchState(FadingOutState);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthController>().Health -= playerDamageAmount;

            if (currentState != KnockedBackState)
            {
                Vector2 directionFromPlayerToEnemy = GetDirectionFromPlayerToEnemy();
                ApplyKnockback(directionFromPlayerToEnemy, playerKnockbackForce);
            }
        }
    }

    public void ApplyKnockback(Vector2 normalizedDirection, float force)
    {
        if (currentState == KnockedBackState)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
        else if (currentState != FadingOutState)
        {
            SwitchState(KnockedBackState);
        }

        rigidbody2D.AddForce(normalizedDirection * force, ForceMode2D.Impulse);

        KnockedBackState.ResetKnockbackStartTime();
    }

    public void DisableCollider()
    {
        collider2D.enabled = false;
    }

    public bool ShouldStartChasingPlayer() =>
        GetDistanceToPlayerCollider() <= maxStartChasingDistanceToPlayer;

    public bool ShouldStopChasingPlayer() =>
        GetDistanceToPlayerCollider() >= minStopChasingDistanceToPlayer;

    public float GetNewFadingOutAlpha() =>
        spriteRenderer.color.a - fadeOutSpeed * Time.deltaTime;

    private Vector2 GetPlayerColliderPosition() =>
        (Vector2)playerTransform.position + playerColliderOffset;

    private Vector2 GetDirectionFromPlayerToEnemy() =>
        ((Vector2)transform.position - GetPlayerColliderPosition()).normalized;

    private float GetDistanceToPlayerCollider() =>
        Vector2.Distance(transform.position, GetPlayerColliderPosition());

    public void SetPlayerInventory(Inventory playerInventory) =>
        this.playerInventory = playerInventory;

    public void SwitchState(BaseEnemyState newState)
    {
        currentState = newState;
        currentState.StateEnter();
    }
}
