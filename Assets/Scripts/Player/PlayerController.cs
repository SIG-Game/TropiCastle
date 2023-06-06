﻿using System;
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
    [SerializeField] private CoconutItemUsage coconutItemUsage;
    [SerializeField] private FishingRodItemUsage fishingRodItemUsage;
    [SerializeField] private RockItemUsage rockItemUsage;

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

    private Dictionary<string, IItemUsage> itemNameToUsage;

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
    private InputAction attackAction;
    private InputAction healAction;
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

        attackAction = attackActionReference.action;
        healAction = healActionReference.action;

        interactableMask = LayerMask.GetMask("Interactable");
        waterMask = LayerMask.GetMask("Water");

        IsAttacking = false;

        ActionDisablingUIOpen = false;

        SetItemNameToUsageDictionary();

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;
        inventory.OnItemChangedAtIndex += Inventory_ChangedItemAtIndex;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            if (healAction.WasPressedThisFrame() && InventoryUIController.InventoryUIOpen)
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
        else if (attackAction.WasPressedThisFrame() && strongestWeaponInInventory != null)
        {
            AttackWithWeapon(strongestWeaponInInventory);
        }
        // Do not check for fish input on the same frame that an item is used
        else if (InputManager.Instance.GetFishButtonDownIfUnusedThisFrame() &&
            !ActionDisablingUIOpen &&
            TryGetFishingRodItemIndex(out int fishingRodItemIndex))
        {
            fishingRodItemUsage.UseItem(fishingRodItemIndex);
        }
        else if (healAction.WasPressedThisFrame())
        {
            ConsumeFirstHealingItemInInventory();
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

    public RaycastHit2D InteractionCast(LayerMask mask, float horizontalBoxCastDistance,
        float verticalBoxCastDistance)
    {
        Vector2 interactionDirection = GetInteractionDirection();

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
                    itemUsage.UseItem();
                }
                break;
        }
    }

    private void ConsumeHealingItem(int itemIndex, int amountToHeal)
    {
        if (!healthController.AtMaxHealth())
        {
            healthController.IncreaseHealth(amountToHeal);
            inventory.DecrementItemStackAtIndex(itemIndex);
        }
    }

    private void AttackWithWeapon(WeaponItemScriptableObject weaponItemData)
    {
        animator.SetFloat("Attack Speed Multiplier", weaponItemData.attackSpeed);

        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.Damage = weaponItemData.damage;
        weaponController.EnemyKnockbackForce = weaponItemData.knockback;

        IsAttacking = true;

        itemSelectionController.CanSelect = false;

        string attackTypeString = weaponItemData.attackType.ToString();

        animator.Play($"{attackTypeString} {Direction}");
    }

    public void Fish()
    {
        if (InteractionCast(waterMask, 0.5f, 0.4f).collider == null)
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

    private void SetItemNameToUsageDictionary()
    {
        itemNameToUsage = new Dictionary<string, IItemUsage>
        {
            { "Bucket", bucketItemUsage },
            { "Coconut", coconutItemUsage },
            { "Fishing Rod", fishingRodItemUsage },
            { "Rock", rockItemUsage }
        };
    }

    private void AttackEndedAnimationEvent()
    {
        IsAttacking = false;

        itemSelectionController.CanSelect = true;
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
