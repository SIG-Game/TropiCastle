using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IInventoryGetter
{
    public enum Direction { Up, Down, Left, Right };

    public HotbarUIController hotbarUIController;

    public bool isAttacking = false;

    public FishingMinigame fishingGame;

    public Direction lastDirection { get; set; }

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

        // TODO: Resources.Load calls should maybe use Addressables instead
        ItemScriptableObject emptyItemInfo = Resources.Load<ItemScriptableObject>("Items/Empty");
        inventory = new Inventory(UseItem, emptyItemInfo);

        lastDirection = Direction.Down;

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
        switch (lastDirection)
        {
            case Direction.Up:
                return Vector2.up;

            case Direction.Down:
                return Vector2.down;

            case Direction.Left:
                return Vector2.left;

            case Direction.Right:
                return Vector2.right;
        }

        // Should not be reached
        return Vector2.zero;
    }

    private void UseItem(ItemWithAmount item)
    {
        if (item.itemData is HealingItemScriptableObject healingItemData)
        {
            ConsumeHealingItem(item, healingItemData.healAmount);
        }
        else if (item.itemData is WeaponItemScriptableObject weaponItemData)
        {
            AttackWithWeapon(weaponItemData);
        }
        else if (item.itemData.name == "FishingRod")
        {
            if (InteractionCast(waterMask, 0.5f).collider == null)
            {
                DialogueBox.Instance.PlayDialogue("You must be facing water to fish.");
                return;
            }

            StartCoroutine(fishingGame.StartFishing());
        }
        else
        {
            Debug.Log($"Used item named {item.itemData.name}, which has no usage defined.");
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
