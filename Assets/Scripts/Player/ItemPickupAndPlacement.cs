using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Sprite cannotPickUpArrow;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;
    [SerializeField] private InputActionReference itemPickupAndPlacementActionReference;

    public bool WaitingForInputReleaseBeforePlacement { get; set; }
    public ItemWorld HoveredItemWorld { get; private set; }
    public bool CanPlaceItemAtCursorPosition { get; private set; }
    public bool CursorIsOverItemWorld { get; private set; }
    public bool PlacingItem { get; private set; }

    public DefaultItemPickupAndPlacementState DefaultState { get; private set; }
    public ItemPickupState PickupState { get; private set; }
    public ItemPlacementState PlacementState { get; private set; }

    private BaseItemPickupAndPlacementState currentState;
    private Vector2 cursorPoint;
    private InputAction itemPickupAndPlacementAction;

    private void Awake()
    {
        WaitingForInputReleaseBeforePlacement = false;

        itemPickupAndPlacementAction = itemPickupAndPlacementActionReference.action;
    }

    private void Start()
    {
        DefaultState = new DefaultItemPickupAndPlacementState(this);
        PickupState = new ItemPickupState(this, itemPickupAndPlacementAction);
        PlacementState = new ItemPlacementState(this, inputManager,
            itemPickupAndPlacementAction);

        currentState = DefaultState;

        currentState.StateEnter();
    }

    private void Update()
    {
        if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            WaitingForInputReleaseBeforePlacement = false;
        }

        if (PauseController.Instance.GamePaused ||
            playerActionDisablingUIManager.ActionDisablingUIOpen || player.IsAttacking)
        {
            bool shouldCancelPlacement = currentState == PlacementState;
            if (shouldCancelPlacement)
            {
                SwitchState(DefaultState);
            }

            return;
        }

        cursorPoint = cursorController.GetWorldPosition();
        UpdateHoveredItemWorld();
        UpdateCanPlaceItemAtCursorPosition();
        UpdatePlacingItem();

        currentState.StateUpdate();
    }

    public void PickUpHoveredItem()
    {
        if (!HoveredItemContainsItem())
        {
            PickUpItemWorld(HoveredItemWorld,
                playerInventory, player.GetSelectedItemIndex());
        }
    }

    public void PlaceSelectedItemAtCursorPosition()
    {
        ItemStack selectedItem = player.GetSelectedItem();

        ItemStack itemToPlace = selectedItem.itemDefinition.OneAtATimePlacement ?
            selectedItem.GetCopyWithAmount(1) : selectedItem;

        int itemToPlaceIndex = player.GetSelectedItemIndex();

        if (!itemToPlace.itemDefinition.IsEmpty())
        {
            Vector2 itemPlacementPosition;

            if (itemToPlace.itemDefinition.LockPlacementToGrid)
            {
                itemPlacementPosition = new Vector2(
                    RoundToGrid(cursorPoint.x), RoundToGrid(cursorPoint.y));
            }
            else
            {
                itemPlacementPosition = cursorPoint;
            }

            _ = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                itemPlacementPosition, itemToPlace);

            if (selectedItem.itemDefinition.OneAtATimePlacement)
            {
                playerInventory.DecrementItemStackAtIndex(itemToPlaceIndex);
            }
            else
            {
                playerInventory.RemoveItemAtIndex(itemToPlaceIndex);
            }
        }
    }

    public void ResetCursor()
    {
        cursorController.UseDefaultCursor();
        cursorController.LockToGrid = false;
    }

    public void ResetCursorAndPlayerItem()
    {
        ResetCursor();
        playerItemInWorld.Hide();
    }

    public void UsePickupCursor()
    {
        bool cannotAddHoveredItem = HoveredItemWorld != null &&
            !playerInventory.CanAddItem(HoveredItemWorld.GetItem(), out int canAddAmount) &&
            canAddAmount == 0;

        if (cannotAddHoveredItem || HoveredItemContainsItem())
        {
            cursorController.Sprite = cannotPickUpArrow;
        }
        else
        {
            cursorController.Sprite = itemPickupArrow;
        }
    }

    public void UsePlacementCursorAndPlayerItem()
    {
        ItemStack selectedItem = player.GetSelectedItem();

        ItemStack placementItem = selectedItem.itemDefinition.OneAtATimePlacement ?
            selectedItem.GetCopyWithAmount(1) : selectedItem;

        cursorController.LockToGrid = selectedItem.itemDefinition.LockPlacementToGrid;

        Color cursorBackgroundColor = CanPlaceItemAtCursorPosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItem.itemDefinition);

        cursorController.UpdateUsingItem(placementItem);
        cursorController.UpdateCursorBackground(cursorBackgroundColor,
            selectedItemColliderSize);

        playerItemInWorld.ShowItem(placementItem);
    }

    public bool SelectedItemIsEmpty()
    {
        ItemScriptableObject selectedItemDefinition =
            player.GetSelectedItem().itemDefinition;

        return selectedItemDefinition.IsEmpty();
    }

    private void UpdateHoveredItemWorld()
    {
        Collider2D cursorOverlapCollider = Physics2D.OverlapPoint(cursorPoint);

        bool cursorIsOverCollider = cursorOverlapCollider != null;
        CursorIsOverItemWorld = cursorIsOverCollider && cursorOverlapCollider.CompareTag("Item World");

        if (CursorIsOverItemWorld)
        {
            HoveredItemWorld = cursorOverlapCollider.GetComponent<ItemWorld>();
        }
        else
        {
            HoveredItemWorld = null;
        }
    }

    private void UpdateCanPlaceItemAtCursorPosition()
    {
        ItemScriptableObject selectedItemDefinition =
            player.GetSelectedItem().itemDefinition;

        Vector2 selectedItemColliderExtents =
            ItemWorldPrefabInstanceFactory.GetItemColliderExtents(selectedItemDefinition);

        Vector2 itemPlacementPosition;

        if (selectedItemDefinition.LockPlacementToGrid)
        {
            itemPlacementPosition = new Vector2(
                RoundToGrid(cursorPoint.x), RoundToGrid(cursorPoint.y));
        }
        else
        {
            itemPlacementPosition = cursorPoint;
        }

        CanPlaceItemAtCursorPosition = SpawnColliderHelper.CanSpawnColliderAtPosition(
            itemPlacementPosition, selectedItemColliderExtents);
    }

    private void UpdatePlacingItem()
    {
        PlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !WaitingForInputReleaseBeforePlacement &&
            !SelectedItemIsEmpty();
    }

    private bool HoveredItemContainsItem() => HoveredItemWorld != null &&
        HoveredItemWorld.TryGetComponent(out Inventory hoveredInventory) &&
        hoveredInventory.GetItemList().Exists(x => !x.itemDefinition.IsEmpty());

    public void SwitchState(BaseItemPickupAndPlacementState newState)
    {
        currentState.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }

    public static void PickUpItemWorld(ItemWorld itemWorld,
        Inventory inventory, int preferredIndex)
    {
        ItemStack item = itemWorld.GetItem();

        inventory.TryAddItemToFirstStackOrIndex(
            item, preferredIndex, out int amountAdded);

        if (amountAdded == item.amount)
        {
            Destroy(itemWorld.gameObject);
        }
        else if (amountAdded != 0)
        {
            itemWorld.SetItemAmount(item.amount - amountAdded);
        }
    }

    // Round to the nearest number ending in .25 or .75
    private static float RoundToGrid(float value) =>
        Mathf.Round(2f * value - 0.5f) / 2f + 0.25f;
}
