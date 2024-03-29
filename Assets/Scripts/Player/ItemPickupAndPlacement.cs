using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Sprite cannotPickUpArrow;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;
    [SerializeField] private InputActionReference itemPickupAndPlacementActionReference;

    [Inject] private CursorController cursorController;
    [Inject] private InputManager inputManager;
    [Inject] private ItemSelectionController itemSelectionController;
    [Inject] private ItemWorldPrefabInstanceFactory itemWorldPrefabInstanceFactory;
    [Inject] private PauseController pauseController;
    [Inject] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [Inject] private PlayerController playerController;
    [Inject("PlayerInventory")] private Inventory playerInventory;
    [Inject("PlayerItemInWorld")] private CharacterItemInWorldController playerItemInWorld;

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
        this.InjectDependencies();

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

        if (pauseController.GamePaused ||
            playerActionDisablingUIManager.ActionDisablingUIOpen ||
            playerController.IsAttacking)
        {
            bool shouldCancelPlacement = currentState == PlacementState;
            if (shouldCancelPlacement)
            {
                SwitchState(DefaultState);
            }

            return;
        }

        cursorPoint = cursorController.GetWorldPosition();

        Collider2D cursorOverlapCollider = Physics2D.OverlapPoint(cursorPoint);

        CursorIsOverItemWorld = cursorOverlapCollider != null &&
            cursorOverlapCollider.CompareTag("Item World");

        HoveredItemWorld = CursorIsOverItemWorld ?
            cursorOverlapCollider.GetComponent<ItemWorld>() : null;

        ItemScriptableObject selectedItemDefinition = GetSelectedItem().ItemDefinition;

        Vector2 selectedItemColliderExtents = ItemWorldPrefabInstanceFactory
            .GetItemColliderExtents(selectedItemDefinition);

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

        PlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !WaitingForInputReleaseBeforePlacement &&
            !SelectedItemIsEmpty();

        currentState.StateUpdate();
    }

    public void PickUpHoveredItem()
    {
        if (!HoveredItemContainsItem())
        {
            PickUpItemWorld(HoveredItemWorld, playerInventory,
                itemSelectionController.SelectedItemIndex);
        }
    }

    public void PlaceSelectedItemAtCursorPosition()
    {
        ItemStack selectedItem = GetSelectedItem();

        ItemStack itemToPlace = selectedItem.ItemDefinition.OneAtATimePlacement ?
            selectedItem.GetCopyWithAmount(1) : selectedItem;

        if (!itemToPlace.ItemDefinition.IsEmpty())
        {
            Vector2 itemPlacementPosition;

            if (itemToPlace.ItemDefinition.LockPlacementToGrid)
            {
                itemPlacementPosition = new Vector2(
                    RoundToGrid(cursorPoint.x), RoundToGrid(cursorPoint.y));
            }
            else
            {
                itemPlacementPosition = cursorPoint;
            }

            _ = itemWorldPrefabInstanceFactory.SpawnItemWorld(
                itemPlacementPosition, itemToPlace);

            if (selectedItem.ItemDefinition.OneAtATimePlacement)
            {
                playerInventory.DecrementItemStackAtIndex(
                    itemSelectionController.SelectedItemIndex);
            }
            else
            {
                playerInventory.RemoveItemAtIndex(
                    itemSelectionController.SelectedItemIndex);
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
            !playerInventory.CanAddItem(HoveredItemWorld.Item, out int canAddAmount) &&
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
        ItemStack selectedItem = GetSelectedItem();

        ItemStack placementItem = selectedItem.ItemDefinition.OneAtATimePlacement ?
            selectedItem.GetCopyWithAmount(1) : selectedItem;

        cursorController.LockToGrid = selectedItem.ItemDefinition.LockPlacementToGrid;

        Color cursorBackgroundColor = CanPlaceItemAtCursorPosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItem.ItemDefinition);

        cursorController.UpdateUsingItem(placementItem);
        cursorController.UpdateCursorBackground(cursorBackgroundColor,
            selectedItemColliderSize);

        playerItemInWorld.ShowItem(placementItem);
    }

    private bool HoveredItemContainsItem() => HoveredItemWorld != null &&
        HoveredItemWorld.TryGetComponent(out Inventory hoveredInventory) &&
        hoveredInventory.GetItemList().Exists(x => !x.ItemDefinition.IsEmpty());

    private ItemStack GetSelectedItem() =>
        playerInventory.GetItemAtIndex(itemSelectionController.SelectedItemIndex);

    public bool SelectedItemIsEmpty() =>
        GetSelectedItem().ItemDefinition.IsEmpty();

    public void SwitchState(BaseItemPickupAndPlacementState newState)
    {
        currentState.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }

    public static void PickUpItemWorld(ItemWorld itemWorld,
        Inventory inventory, int preferredIndex)
    {
        ItemStack item = itemWorld.Item;

        inventory.TryAddItemToFirstStackOrIndex(
            item, preferredIndex, out int amountAdded);

        if (amountAdded == item.Amount)
        {
            Destroy(itemWorld.gameObject);
        }
        else if (amountAdded != 0)
        {
            itemWorld.SetItemAmount(item.Amount - amountAdded);
        }
    }

    // Round to the nearest number ending in .25 or .75
    private static float RoundToGrid(float value) =>
        Mathf.Round(2f * value - 0.5f) / 2f + 0.25f;
}
