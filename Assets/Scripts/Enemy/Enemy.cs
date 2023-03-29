﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int playerDamageAmount;
    [SerializeField] private float playerKnockbackForce;
    [SerializeField] private float maxStartChasingDistanceToPlayer;
    [SerializeField] private float minStopChasingDistanceToPlayer;
    [SerializeField] private float initialWaitTimeBeforeChilling;
    [SerializeField] private float waitTimeAfterKnockback;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<ItemWithAmount> droppedLoot;

    private Rigidbody2D rb2d;
    private HealthController healthController;
    private Vector2 velocityDirection;
    private Vector2 playerColliderOffset;
    private EnemyState state;

    public float LastHitTime { get; set; }

    private void Awake()
    {
        LastHitTime = 0f;

        rb2d = GetComponent<Rigidbody2D>();
        healthController = GetComponent<HealthController>();

        state = EnemyState.Initial;

        StartCoroutine(InitialWaitBeforeChillingCoroutine());

        healthController.OnHealthChanged += HealthController_OnHealthChanged;
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
            velocityDirection = Vector2.zero;

            if (Vector2.Distance(transform.position, GetPlayerColliderPosition()) <= maxStartChasingDistanceToPlayer)
            {
                state = EnemyState.Chasing;
            }
        }

        if (state == EnemyState.Chasing)
        {
            Vector2 fromEnemyToPlayerPosition = GetPlayerColliderPosition() - (Vector2)transform.position;

            float distanceToPlayer = fromEnemyToPlayerPosition.magnitude;

            if (distanceToPlayer >= minStopChasingDistanceToPlayer)
            {
                state = EnemyState.Chilling;
            }
            else
            {
                Vector2 directionToPlayer = fromEnemyToPlayerPosition.normalized;
                velocityDirection = directionToPlayer;
            }
        }
    }

    private void FixedUpdate()
    {
        if (velocityDirection != Vector2.zero)
        {
            rb2d.MovePosition((Vector2)transform.position + velocityDirection * speed);
        }
    }

    private void HealthController_OnHealthChanged(int newHealth)
    {
        if (newHealth <= 0)
        {
            // TODO: This doesn't work properly for loot items with amount value > 1
            // This should be revisited if stackable items get added
            foreach (ItemWithAmount loot in droppedLoot)
            {
                ItemWorldPrefabInstanceFactory.Instance.DropItem(transform.position, loot);
            }
            Destroy(gameObject);
        }
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
            return;
        }

        velocityDirection = Vector2.zero;

        state = EnemyState.KnockedBack;

        transform.GetComponent<Rigidbody2D>().AddForce(normalizedDirection * force, ForceMode2D.Impulse);

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
        state = EnemyState.Chasing;
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private Vector2 GetPlayerColliderPosition()
    {
        Vector2 playerColliderPosition = (Vector2)playerTransform.position + playerColliderOffset;
        return playerColliderPosition;
    }

    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }
}