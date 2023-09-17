﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ISavablePrefabInstance
{
    [SerializeField] private float speed;
    [SerializeField] private int playerDamageAmount;
    [SerializeField] private float playerKnockbackForce;
    [SerializeField] private float maxStartChasingDistanceToPlayer;
    [SerializeField] private float minStopChasingDistanceToPlayer;
    [SerializeField] private float initialWaitTimeBeforeIdleSeconds;
    [SerializeField] private float waitTimeAfterKnockbackSeconds;
    [SerializeField] private float fadeOutSpeed;
    [SerializeField] private CanvasGroup healthBarCanvasGroup;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private List<ItemWithAmount> droppedLoot;

    public InitialEnemyState InitialState { get; private set; }
    public IdleEnemyState IdleState { get; private set; }
    public ChasingEnemyState ChasingState { get; private set; }
    public KnockedBackEnemyState KnockedBackState { get; private set; }
    public FadingOutEnemyState FadingOutState { get; private set; }

    public float LastHitTime { get; set; }

    private BaseEnemyState currentState;
    private Rigidbody2D rb2d;
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private HealthController healthController;
    private Spawnable spawnable;
    private Vector2 playerColliderOffset;

    private void Awake()
    {
        InitialState = new InitialEnemyState(this);
        IdleState = new IdleEnemyState(this);
        ChasingState = new ChasingEnemyState(this);
        KnockedBackState = new KnockedBackEnemyState(this);
        FadingOutState = new FadingOutEnemyState(this);

        LastHitTime = 0f;

        rb2d = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthController = GetComponent<HealthController>();
        spawnable = GetComponent<Spawnable>();

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

    public void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if (healthBarCanvasGroup != null)
        {
            healthBarCanvasGroup.alpha = alpha;
        }
    }

    public void EnemyDeath()
    {
        foreach (ItemWithAmount loot in droppedLoot)
        {
            playerInventory.TryAddItem(loot, out int amountAdded);

            bool lootItemNotFullyAdded = amountAdded != loot.amount;
            if (lootItemNotFullyAdded)
            {
                ItemWithAmount itemToDrop = new ItemWithAmount(loot.itemDefinition,
                    loot.amount - amountAdded, loot.instanceProperties);

                ItemWorldPrefabInstanceFactory.Instance.DropItem(
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
            HealthController playerHealthController = other.GetComponent<HealthController>();
            playerHealthController.DecreaseHealth(playerDamageAmount);

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
            rb2d.velocity = Vector2.zero;
        }
        else if (currentState != FadingOutState)
        {
            SwitchState(KnockedBackState);
        }

        rb2d.AddForce(normalizedDirection * force, ForceMode2D.Impulse);

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

    public float GetInitialWaitTimeBeforeIdleSeconds() =>
        initialWaitTimeBeforeIdleSeconds;

    public float GetWaitTimeAfterKnockbackSeconds() =>
        waitTimeAfterKnockbackSeconds;

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

    public void SwitchState(BaseEnemyState newState)
    {
        currentState = newState;
        currentState.StateEnter();
    }

    public SavablePrefabInstanceState GetSavablePrefabInstanceState()
    {
        var savableState = new SavableEnemyState
        {
            Position = transform.position,
            Health = healthController.CurrentHealth,
            SpawnerGuid = spawnable.GetSpawnerGuid()
        };

        return savableState;
    }

    public void SetPropertiesFromSavablePrefabInstanceState(
        SavablePrefabInstanceState savableState)
    {
        SavableEnemyState enemyState = (SavableEnemyState)savableState;

        transform.position = enemyState.Position;

        healthController.CurrentHealth = enemyState.Health;

        GetComponent<Spawnable>()
            .SetSpawnerUsingGuid<EnemySpawner>(enemyState.SpawnerGuid);
    }

    [Serializable]
    public class SavableEnemyState : SavablePrefabInstanceState
    {
        public Vector2 Position;
        public int Health;
        public string SpawnerGuid;

        public override string GetSavablePrefabName() => "Crab";
    }
}
