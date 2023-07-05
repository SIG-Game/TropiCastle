using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private SpriteMask overlaySpriteMask;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private InputActionReference attackActionReference;
    [SerializeField] private InputActionReference healActionReference;

    [Header("Item Usages")]
    [SerializeField] private BucketItemUsage bucketItemUsage;
    [SerializeField] private FishingRodItemUsage fishingRodItemUsage;
    [SerializeField] private HealingItemUsage healingItemUsage;
    [SerializeField] private ThrowableItemUsage throwableItemUsage;
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

    public event Action<bool> OnIsAttackingSet = delegate { };
    public event Action OnPlayerDied = delegate { };

    private Dictionary<string, IItemUsage> itemNameToUsage;
    private Dictionary<Type, IItemUsage> itemScriptableObjectTypeToUsage;

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

    public static event Action<bool> OnActionDisablingUIOpenSet = delegate { };

    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private ItemSelectionController itemSelectionController;
    private SpriteRenderer spriteRenderer;
    private CharacterDirectionController directionController;
    private InputAction attackAction;
    private InputAction healAction;
    private LayerMask interactableMask;
    private bool isAttacking;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        itemSelectionController = GetComponent<ItemSelectionController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        directionController = GetComponent<CharacterDirectionController>();

        attackAction = attackActionReference.action;
        healAction = healActionReference.action;

        interactableMask = LayerMask.GetMask("Interactable");

        SetItemUsageDictionaries();

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;
    }

    private void Start()
    {
        IsAttacking = false;

        ActionDisablingUIOpen = false;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            if (healAction.WasPressedThisFrame() && InventoryUIController.InventoryUIOpen)
            {
                healingItemUsage.ConsumeFirstHealingItemInPlayerInventory();
            }

            return;
        }

        if (InputManager.Instance.GetUseItemButtonDownIfUnusedThisFrame())
        {
            UseItem(GetSelectedItem(), GetSelectedItemIndex());

            // Prevent doing other actions on the frame an attack starts
            if (IsAttacking)
            {
                return;
            }
        }
        else if (attackAction.WasPressedThisFrame() && weaponItemUsage.PlayerHasWeapon())
        {
            weaponItemUsage.AttackWithStrongestWeaponInInventory();
        }
        // Do not check for fish input on the same frame that an item is used
        else if (InputManager.Instance.GetFishButtonDownIfUnusedThisFrame() &&
            !ActionDisablingUIOpen &&
            TryGetFishingRodItemIndex(out int fishingRodItemIndex))
        {
            ItemWithAmount fishingRodItem = inventory.GetItemList()[fishingRodItemIndex];

            fishingRodItemUsage.UseItem(fishingRodItem, fishingRodItemIndex);
        }
        else if (healAction.WasPressedThisFrame())
        {
            healingItemUsage.ConsumeFirstHealingItemInPlayerInventory();
        }

        if (InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame())
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

        OnActionDisablingUIOpenSet = delegate { };
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

    private void UseItem(ItemWithAmount item, int itemIndex)
    {
        if (TryGetItemUsage(item, out IItemUsage itemUsage))
        {
            itemUsage.UseItem(item, itemIndex);
        }
    }

    private bool TryGetItemUsage(ItemWithAmount item, out IItemUsage itemUsage) =>
        itemNameToUsage.TryGetValue(item.itemData.name, out itemUsage) ||
        itemScriptableObjectTypeToUsage.TryGetValue(item.itemData.GetType(), out itemUsage);

    private void PlayerDeath()
    {
        PauseController.Instance.GamePaused = true;
        OnPlayerDied();
    }

    private bool TryGetFishingRodItemIndex(out int fishingRodItemIndex)
    {
        fishingRodItemIndex = inventory.GetItemList()
            .FindIndex(x => x.itemData.name == "Fishing Rod");

        return fishingRodItemIndex != -1;
    }

    private void HealthController_OnHealthSetToZero()
    {
        PlayerDeath();
    }

    private void SetItemUsageDictionaries()
    {
        itemNameToUsage = new Dictionary<string, IItemUsage>
        {
            { "Bucket", bucketItemUsage },
            { "Fishing Rod", fishingRodItemUsage }
        };

        itemScriptableObjectTypeToUsage = new Dictionary<Type, IItemUsage>
        {
            { typeof(HealingItemScriptableObject), healingItemUsage },
            { typeof(ThrowableItemScriptableObject), throwableItemUsage },
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

    public SerializablePlayerProperties GetSerializablePlayerProperties()
    {
        Vector2 playerPosition = transform.position;
        int playerDirection = (int)Direction;
        int selectedItemIndex = GetSelectedItemIndex();
        int health = healthController.CurrentHealth;

        var serializablePlayerProperties = new SerializablePlayerProperties
        {
            PlayerPosition = playerPosition,
            PlayerDirection = playerDirection,
            SelectedItemIndex = selectedItemIndex,
            Health = health
        };

        return serializablePlayerProperties;
    }

    public void SetPropertiesFromSerializablePlayerProperties(
        SerializablePlayerProperties serializablePlayerProperties)
    {
        transform.position = serializablePlayerProperties.PlayerPosition;
        Direction = (CharacterDirection)serializablePlayerProperties.PlayerDirection;
        itemSelectionController.SelectedItemIndex = serializablePlayerProperties.SelectedItemIndex;
        healthController.CurrentHealth = serializablePlayerProperties.Health;
    }

    [Serializable]
    public class SerializablePlayerProperties
    {
        public Vector2 PlayerPosition;
        public int PlayerDirection;
        public int SelectedItemIndex;
        public int Health;
    }

    public bool CanMove() => !IsAttacking && !PauseController.Instance.GamePaused && !ActionDisablingUIOpen;

    public int GetSelectedItemIndex() => itemSelectionController.SelectedItemIndex;

    public ItemWithAmount GetSelectedItem() => inventory.GetItemAtIndex(itemSelectionController.SelectedItemIndex);

    public Inventory GetInventory() => inventory;
}
