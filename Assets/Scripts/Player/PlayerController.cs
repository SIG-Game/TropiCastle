using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IInventoryGetter
{
    public HotbarUIController hotbarUIController;

    public bool isAttacking = false;

    public FishingMinigame fishingGame;

    public PlayerDirection lastDirection { get; set; }

    public static event Action OnPlayerDied = delegate { };

    private Animator animator;
    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private LayerMask interactableMask;
    private LayerMask waterMask;
    private SpriteRenderer weaponSpriteRenderer;
    private WeaponController weaponController;

    private Inventory inventory;

    void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();

        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        weaponController = transform.GetChild(0).GetComponent<WeaponController>();

        inventory = new Inventory();

        lastDirection = PlayerDirection.Down;

        interactableMask = LayerMask.GetMask("Interactable");
        waterMask = LayerMask.GetMask("Water");

        healthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (InputManager.Instance.GetLeftClickDownIfUnusedThisFrame())
        {
            UseItem(GetHotbarItem());

            // Prevent doing other actions on the frame an attack starts
            if (isAttacking)
            {
                return;
            }
        }

        if (InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame())
        {
            RaycastHit2D hit = InteractionCast(interactableMask, 0.15f);

            if (hit.collider != null)
            {
                hit.transform.gameObject.GetComponent<Interactable>().Interact(this);
            }
        }
    }

    private void OnDestroy()
    {
        OnPlayerDied = delegate { };
    }

    private RaycastHit2D InteractionCast(LayerMask mask, float boxCastDistance)
    {
        Vector2 interactionDirection = GetInteractionDirection();

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.x += boxCollider.offset.x;
        raycastOrigin.y += boxCollider.offset.y;

        // TODO: Shorten length and change length based on direction
        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin,
            boxCollider.size, 0f, interactionDirection, boxCastDistance, mask);

        // Debug.DrawRay(raycastOrigin, interactionDirection * 0.25f, Color.red);

        return hit;
    }

    public bool CanMove()
    {
        return !isAttacking && !PauseController.Instance.GamePaused && !DialogueBox.Instance.DialogueBoxOpen();
    }

    public void PlayerDeath()
    {
        PauseController.Instance.GamePaused = true;
        OnPlayerDied();
    }

    public void AttackWithWeapon(WeaponItemScriptableObject weaponItemData) {
        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.damage = weaponItemData.damage;

        animator.Play($"Swing {lastDirection}");

        isAttacking = true;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    Vector2 GetInteractionDirection()
    {
        Vector2 interactionDirection = lastDirection switch
        {
            PlayerDirection.Up => Vector2.up,
            PlayerDirection.Down => Vector2.down,
            PlayerDirection.Left => Vector2.left,
            PlayerDirection.Right => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(nameof(lastDirection))
        };
        return interactionDirection;
    }

    private void UseItem(ItemWithAmount item)
    {
        switch (item.itemData)
        {
            case HealingItemScriptableObject healingItemData:
                ConsumeHealingItem(item, healingItemData.healAmount);
                break;
            case WeaponItemScriptableObject weaponItemData:
                AttackWithWeapon(weaponItemData);
                break;
            case { name: "FishingRod" }:
                if (InteractionCast(waterMask, 0.5f).collider == null)
                {
                    DialogueBox.Instance.PlayDialogue("You must be facing water to fish.");
                    break;
                }

                StartCoroutine(fishingGame.StartFishing());
                break;
            default:
                Debug.Log($"Used item named {item.itemData.name}, which has no usage defined.");
                break;
        }
    }

    private void ConsumeHealingItem(ItemWithAmount item, int amountToHeal)
    {
        if (!healthController.AtMaxHealth())
        {
            healthController.IncreaseHealth(amountToHeal);
            inventory.RemoveItem(item);
        }
    }

    private void HealthController_OnHealthChanged(int newHealth)
    {
        if (newHealth == 0)
        {
            PlayerDeath();
        }
    }

    public ItemWithAmount GetHotbarItem()
    {
        return inventory.GetItemAtIndex(hotbarUIController.HotbarItemIndex);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Bullet"))
        {
            healthController.DecreaseHealth(10);
        }
    }
}
