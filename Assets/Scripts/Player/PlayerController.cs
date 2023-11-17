using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private SpriteMask overlaySpriteMask;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InputActionReference healActionReference;

    [Header("Item Usages")]
    [SerializeField] private FishingRodItemUsage fishingRodItemUsage;
    [SerializeField] private HealingItemUsage healingItemUsage;
    [SerializeField] private WeaponItemUsage weaponItemUsage;

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

    public event Action<bool> OnIsAttackingSet = (_) => {};
    public event Action OnPlayerDied = () => {};

    private Dictionary<string, IItemUsage> itemNameToUsage;
    private Dictionary<Type, IItemUsage> itemScriptableObjectTypeToUsage;

    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private ItemSelectionController itemSelectionController;
    private SpriteRenderer spriteRenderer;
    private CharacterDirectionController directionController;
    private InputAction healAction;
    private LayerMask interactableMask;
    private LayerMask waterMask;
    private bool isAttacking;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        itemSelectionController = GetComponent<ItemSelectionController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        directionController = GetComponent<CharacterDirectionController>();

        healAction = healActionReference.action;

        interactableMask = LayerMask.GetMask("Interactable");
        waterMask = LayerMask.GetMask("Water");

        SetItemUsageDictionaries();

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;
    }

    private void Start()
    {
        IsAttacking = false;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            if (healAction.WasPressedThisFrame() && inventoryUIManager.InventoryUIOpen)
            {
                healingItemUsage.ConsumeFirstHealingItemInPlayerInventory();
            }

            return;
        }

        if (inputManager.GetUseItemButtonDownIfUnusedThisFrame())
        {
            UseItem(GetSelectedItem(), GetSelectedItemIndex());

            // Prevent doing other actions on the frame an attack starts
            if (IsAttacking)
            {
                return;
            }
        }
        else if (healAction.WasPressedThisFrame())
        {
            healingItemUsage.ConsumeFirstHealingItemInPlayerInventory();
        }

        if (inputManager.GetInteractButtonDownIfUnusedThisFrame())
        {
            RaycastHit2D hit = InteractionCast(interactableMask, 0.15f, 0.12f);

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
            healthController.OnHealthSetToZero -= HealthController_OnHealthSetToZero;
        }
    }

    public RaycastHit2D InteractionCast(LayerMask mask, float horizontalBoxCastDistance,
        float verticalBoxCastDistance)
    {
        Vector2 interactionDirection = directionController.GetDirectionVector();

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.x += boxCollider.offset.x;
        raycastOrigin.y += boxCollider.offset.y;

        float boxCastDistance;

        if (Direction == CharacterDirection.Left || Direction == CharacterDirection.Right)
        {
            boxCastDistance = horizontalBoxCastDistance;
        }
        else
        {
            boxCastDistance = verticalBoxCastDistance;
        }

        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin, boxCollider.size,
            0f, interactionDirection, boxCastDistance, mask);

        // Debug.DrawRay(raycastOrigin, interactionDirection * 0.25f, Color.red);

        return hit;
    }

    public RaycastHit2D WaterInteractionCast(
        float horizontalBoxCastDistance, float verticalBoxCastDistance) =>
        InteractionCast(waterMask, horizontalBoxCastDistance, verticalBoxCastDistance);

    private void UseItem(ItemStack item, int itemIndex)
    {
        if (item.itemDefinition.HasProperty("HealAmount"))
        {
            healingItemUsage.UseItem(item, itemIndex);
        }
        else if (TryGetItemUsage(item, out IItemUsage itemUsage))
        {
            itemUsage.UseItem(item, itemIndex);
        }
    }

    private bool TryGetItemUsage(ItemStack item, out IItemUsage itemUsage) =>
        itemNameToUsage.TryGetValue(item.itemDefinition.name, out itemUsage) ||
        itemScriptableObjectTypeToUsage.TryGetValue(item.itemDefinition.GetType(), out itemUsage);

    private void PlayerDeath()
    {
        PauseController.Instance.GamePaused = true;
        OnPlayerDied();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item World"))
        {
            ItemPickupAndPlacement.PickUpItemWorld(
                other.GetComponent<ItemWorld>(), inventory, GetSelectedItemIndex());
        }
    }

    private void HealthController_OnHealthSetToZero()
    {
        PlayerDeath();
    }

    private void SetItemUsageDictionaries()
    {
        itemNameToUsage = new Dictionary<string, IItemUsage>
        {
            { "Fishing Rod", fishingRodItemUsage }
        };

        itemScriptableObjectTypeToUsage = new Dictionary<Type, IItemUsage>
        {
            { typeof(WeaponItemScriptableObject), weaponItemUsage }
        };
    }

    private void AttackEndedAnimationEvent()
    {
        IsAttacking = false;

        itemSelectionController.CanSelect = true;
    }

    public void DisableItemSelection()
    {
        itemSelectionController.CanSelect = false;
    }

    public bool CanMove() => !IsAttacking && !PauseController.Instance.GamePaused &&
        !playerActionDisablingUIManager.ActionDisablingUIOpen;

    public int GetSelectedItemIndex() => itemSelectionController.SelectedItemIndex;

    public ItemStack GetSelectedItem() => inventory.GetItemAtIndex(itemSelectionController.SelectedItemIndex);

    public Inventory GetInventory() => inventory;
}
