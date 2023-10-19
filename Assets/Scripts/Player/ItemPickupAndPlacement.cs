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
    [SerializeField] private Sprite itemPickupArrowInventoryFull;
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

    public void AttemptToPickUpHoveredItem()
    {
        ItemStack hoveredItem = HoveredItemWorld.GetItem();

        playerInventory.TryAddItemToFirstStackOrIndex(
            hoveredItem, player.GetSelectedItemIndex(), out int amountAdded);

        bool hoveredItemAdded = amountAdded == hoveredItem.amount;
        bool hoveredItemPartiallyAdded = !hoveredItemAdded && amountAdded != 0;
        if (hoveredItemAdded)
        {
            Destroy(HoveredItemWorld.gameObject);
        }
        else if (hoveredItemPartiallyAdded)
        {
            HoveredItemWorld.SetItemAmount(hoveredItem.amount - amountAdded);
        }
    }

    public void PlaceSelectedItemAtCursorPosition()
    {
        ItemStack selectedItem = player.GetSelectedItem();

        ItemStack itemToPlace = selectedItem.itemDefinition.oneAtATimePlacement ?
            selectedItem.GetCopyWithAmount(1) : selectedItem;

        int itemToPlaceIndex = player.GetSelectedItemIndex();

        if (itemToPlace.itemDefinition.name != "Empty")
        {
            Vector2 itemPlacementPosition;

            if (itemToPlace.itemDefinition.lockPlacementToGrid)
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

            if (selectedItem.itemDefinition.oneAtATimePlacement)
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
    }

    public void ResetCursorAndPlayerItem()
    {
        ResetCursor();
        playerItemInWorld.Hide();
    }

    public void UsePickupCursor()
    {
        if (HoveredItemWorld != null &&
            !playerInventory.CanAddItem(HoveredItemWorld.GetItem(), out int canAddAmount) &&
            canAddAmount == 0)
        {
            cursorController.Sprite = itemPickupArrowInventoryFull;
        }
        else
        {
            cursorController.Sprite = itemPickupArrow;
        }
    }

    public void UsePlacementCursorAndPlayerItem()
    {
        ItemStack selectedItem = player.GetSelectedItem();

        ItemStack placementItem = selectedItem.itemDefinition.oneAtATimePlacement ?
            selectedItem.GetCopyWithAmount(1) : selectedItem;

        Color cursorBackgroundColor = CanPlaceItemAtCursorPosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItem.itemDefinition);

        cursorController.UpdateUsingItem(placementItem);
        cursorController.UpdateCursorBackground(cursorBackgroundColor,
            selectedItemColliderSize);

        playerItemInWorld.ShowItem(placementItem);
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

        if (selectedItemDefinition.lockPlacementToGrid)
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
        ItemScriptableObject selectedItemDefinition =
            player.GetSelectedItem().itemDefinition;

        PlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !WaitingForInputReleaseBeforePlacement &&
            selectedItemDefinition.name != "Empty";
    }

    public void SwitchState(BaseItemPickupAndPlacementState newState)
    {
        currentState.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }

    // Round to the nearest number ending in .25 or .75
    private static float RoundToGrid(float value) =>
        Mathf.Round(2f * value - 0.5f) / 2f + 0.25f;
}
