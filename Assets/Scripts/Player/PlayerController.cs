using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private SpriteMask overlaySpriteMask;

    [Header("Item Usages")]
    [SerializeField] private FishingRodItemUsage fishingRodItemUsage;
    [SerializeField] private HealingItemUsage healingItemUsage;
    [SerializeField] private WeaponItemUsage weaponItemUsage;

    [Inject] private InputManager inputManager;
    [Inject] private PauseController pauseController;
    [Inject] private PlayerActionDisablingUIManager playerActionDisablingUIManager;

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

    public bool InWater { get; private set; }

    public event Action<bool> OnIsAttackingSet = (_) => {};
    public event Action OnPlayerDied = () => {};

    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private SpriteRenderer spriteRenderer;
    private CharacterDirectionController directionController;
    private LayerMask interactableMask;
    private LayerMask waterMask;
    private int waterLayer;
    private bool isAttacking;

    private void Awake()
    {
        this.InjectDependencies();

        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        directionController = GetComponent<CharacterDirectionController>();

        interactableMask = LayerMask.GetMask("Interactable");
        waterMask = LayerMask.GetMask("Water");

        waterLayer = LayerMask.NameToLayer("Water");

        healthController.OnHealthSetToZero += HealthController_OnHealthSetToZero;
    }

    private void Start()
    {
        IsAttacking = false;
    }

    private void Update()
    {
        if (IsAttacking || pauseController.GamePaused)
        {
            return;
        }

        if (inputManager.GetUseItemButtonDownIfUnusedThisFrame())
        {
            ItemStack selectedItem = inventory.GetItemAtIndex(
                itemSelectionController.SelectedItemIndex);

            UseItem(selectedItem, itemSelectionController.SelectedItemIndex);

            // Prevent doing other actions on the frame an attack starts
            if (IsAttacking)
            {
                return;
            }
        }

        if (inputManager.GetInteractButtonDownIfUnusedThisFrame())
        {
            RaycastHit2D hit = InteractionCast(interactableMask, 0.15f, 0.12f);

            if (hit.collider != null)
            {
                hit.transform.gameObject.GetComponent<IInteractable>().Interact();
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
        if (item.ItemDefinition.HasProperty("HealAmount"))
        {
            healingItemUsage.UseItem(item, itemIndex);
        }
        else if (item.ItemDefinition.HasProperty("AttackType"))
        {
            weaponItemUsage.UseItem(item, itemIndex);
        }
        else if (item.ItemDefinition.name == "FishingRod")
        {
            fishingRodItemUsage.UseItem(itemIndex);
        }
    }

    private void PlayerDeath()
    {
        pauseController.GamePaused = true;
        OnPlayerDied();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            InWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            InWater = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item World"))
        {
            ItemPickupAndPlacement.PickUpItemWorld(other.GetComponent<ItemWorld>(),
                inventory, itemSelectionController.SelectedItemIndex);
        }
    }

    private void HealthController_OnHealthSetToZero()
    {
        PlayerDeath();
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

    public bool CanMove() => !IsAttacking && !pauseController.GamePaused &&
        !playerActionDisablingUIManager.ActionDisablingUIOpen;
}
