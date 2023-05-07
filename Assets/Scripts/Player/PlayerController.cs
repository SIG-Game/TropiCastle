using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private SpriteMask overlaySpriteMask;

    public CharacterDirection Direction
    {
        get => directionController.Direction;
        set
        {
            directionController.Direction = value;

            overlaySpriteMask.sprite = spriteRenderer.sprite;
        }
    }

    public bool IsAttacking
    {
        get => isAttacking;
        set
        {
            isAttacking = value;
            OnIsAttackingSet(isAttacking);
        }
    }

    public event Action OnFishingRodUsed = delegate { };
    public event Action<bool> OnIsAttackingSet = delegate { };

    private static readonly Dictionary<string, IItemUsage> itemNameToUsage =
        new Dictionary<string, IItemUsage>
    {
        { "Bucket", new BucketItemUsage() },
        { "Fishing Rod", new FishingRodItemUsage() }
    };

    private static bool actionDisablingUIOpen;

    // Not used for pausing menu
    public static bool ActionDisablingUIOpen
    {
        get => actionDisablingUIOpen;
        set
        {
            actionDisablingUIOpen = value;
            OnActionDisablingUIOpenSet(actionDisablingUIOpen);
        }
    }

    public static event Action OnPlayerDied = delegate { };
    public static event Action<bool> OnActionDisablingUIOpenSet = delegate { };

    private Animator animator;
    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private ItemSelectionController itemSelectionController;
    private SpriteRenderer spriteRenderer;
    private CharacterDirectionController directionController;
    private SpriteRenderer weaponSpriteRenderer;
    private WeaponController weaponController;
    private InputAction attackInputAction;
    private InputAction healInputAction;
    private WeaponItemScriptableObject strongestWeaponInInventory;
    private LayerMask interactableMask;
    private LayerMask waterMask;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        itemSelectionController = GetComponent<ItemSelectionController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        directionController = GetComponent<CharacterDirectionController>();

        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        weaponController = transform.GetChild(0).GetComponent<WeaponController>();

        interactableMask = LayerMask.GetMask("Interactable");
        waterMask = LayerMask.GetMask("Water");

        IsAttacking = false;

        ActionDisablingUIOpen = false;

        healthController.OnHealthSet += HealthController_OnHealthSet;
        inventory.OnItemChangedAtIndex += Inventory_ChangedItemAtIndex;
    }

    private void Start()
    {
        attackInputAction = InputManager.Instance.GetAction("Attack");
        healInputAction = InputManager.Instance.GetAction("Heal");
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            if (healInputAction.WasPressedThisFrame() && InventoryUIController.InventoryUIOpen)
            {
                ConsumeFirstHealingItemInInventory();
            }

            return;
        }

        if (InputManager.Instance.GetUseItemButtonDownIfUnusedThisFrame())
        {
            UseItem(GetSelectedItem());

            // Prevent doing other actions on the frame an attack starts
            if (IsAttacking)
            {
                return;
            }
        }
        else if (attackInputAction.WasPressedThisFrame() && strongestWeaponInInventory != null)
        {
            AttackWithWeapon(strongestWeaponInInventory);
        }
        // Do not check for fish input on the same frame that an item is used
        else if (InputManager.Instance.GetFishButtonDownIfUnusedThisFrame() &&
            !ActionDisablingUIOpen &&
            inventory.GetItemList().FindIndex(x => x.itemData.name == "Fishing Rod") != -1)
        {
            Fish();
        }
        else if (healInputAction.WasPressedThisFrame())
        {
            ConsumeFirstHealingItemInInventory();
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
        if (healthController != null)
        {
            healthController.OnHealthSet -= HealthController_OnHealthSet;
        }

        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex -= Inventory_ChangedItemAtIndex;
        }

        OnPlayerDied = delegate { };
        OnActionDisablingUIOpenSet = delegate { };
    }

    private Vector2 GetInteractionDirection()
    {
        Vector2 interactionDirection = Direction switch
        {
            CharacterDirection.Up => Vector2.up,
            CharacterDirection.Down => Vector2.down,
            CharacterDirection.Left => Vector2.left,
            CharacterDirection.Right => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction))
        };
        return interactionDirection;
    }

    public RaycastHit2D InteractionCast(LayerMask mask, float boxCastDistance)
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

    private void UseItem(ItemWithAmount item)
    {
        switch (item.itemData)
        {
            case HealingItemScriptableObject healingItemData:
                ConsumeHealingItem(GetSelectedItemIndex(), healingItemData.healAmount);
                break;
            case WeaponItemScriptableObject weaponItemData:
                AttackWithWeapon(weaponItemData);
                break;
            default:
                if (itemNameToUsage.TryGetValue(item.itemData.name, out IItemUsage itemUsage))
                {
                    itemUsage.UseItem(this);
                }
                break;
        }
    }

    private void ConsumeHealingItem(int itemIndex, int amountToHeal)
    {
        if (!healthController.AtMaxHealth())
        {
            healthController.IncreaseHealth(amountToHeal);
            inventory.RemoveItemAtIndex(itemIndex);
        }
    }

    private void AttackWithWeapon(WeaponItemScriptableObject weaponItemData)
    {
        animator.SetFloat("Attack Speed Multiplier", weaponItemData.attackSpeed);

        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.Damage = weaponItemData.damage;
        weaponController.EnemyKnockbackForce = weaponItemData.knockback;

        string attackTypeString = weaponItemData.attackType.ToString();

        animator.Play($"{attackTypeString} {Direction}");
    }

    public void Fish()
    {
        if (InteractionCast(waterMask, 0.5f).collider == null)
        {
            DialogueBox.Instance.PlayDialogue("You must be facing water to fish.");
            return;
        }

        OnFishingRodUsed();
    }

    private void PlayerDeath()
    {
        PauseController.Instance.GamePaused = true;
        OnPlayerDied();
    }

    public void ConsumeFirstHealingItemInInventory()
    {
        int healingItemIndex = inventory.GetItemList().FindIndex(x => x.itemData is HealingItemScriptableObject);

        if (healingItemIndex != -1)
        {
            HealingItemScriptableObject healingItem = inventory.GetItemAtIndex(healingItemIndex).itemData
                as HealingItemScriptableObject;

            ConsumeHealingItem(healingItemIndex, healingItem.healAmount);
        }
    }

    private void HealthController_OnHealthSet(int newHealth)
    {
        if (newHealth == 0)
        {
            PlayerDeath();
        }
    }

    private void Inventory_ChangedItemAtIndex(ItemWithAmount _, int _1)
    {
        UpdateStrongestWeaponInInventory();
    }

    private void UpdateStrongestWeaponInInventory()
    {
        strongestWeaponInInventory = null;

        foreach (ItemWithAmount item in inventory.GetItemList())
        {
            if (item.itemData is not WeaponItemScriptableObject)
            {
                continue;
            }

            WeaponItemScriptableObject weapon = item.itemData as WeaponItemScriptableObject;

            if (strongestWeaponInInventory == null ||
                weapon.damage > strongestWeaponInInventory.damage)
            {
                strongestWeaponInInventory = weapon;
            }
        }
    }

    private void AttackStartedAnimationEvent()
    {
        IsAttacking = true;

        itemSelectionController.CanSelect = false;
    }

    private void AttackEndedAnimationEvent()
    {
        IsAttacking = false;

        itemSelectionController.CanSelect = true;
    }

    public bool CanMove() => !IsAttacking && !PauseController.Instance.GamePaused && !ActionDisablingUIOpen;

    public int GetSelectedItemIndex() => itemSelectionController.SelectedItemIndex;

    public ItemWithAmount GetSelectedItem() => inventory.GetItemAtIndex(itemSelectionController.SelectedItemIndex);

    public Inventory GetInventory() => inventory;
}
