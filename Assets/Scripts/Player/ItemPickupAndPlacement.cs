using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Sprite itemPickupArrowInventoryFull;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;
    [SerializeField] private InputActionReference itemPickupAndPlacementActionReference;

    public bool WaitingForInputReleaseBeforePlacement { get; set; }

    public DefaultItemPickupAndPlacementState DefaultState { get; private set; }
    public ItemPickupState PickupState { get; private set; }
    public ItemPlacementState PlacementState { get; private set; }

    private BaseItemPickupAndPlacementState currentState;
    private Inventory playerInventory;
    private Vector2 cursorPoint;
    private ItemWorld hoveredItemWorld;
    private InputAction itemPickupAndPlacementAction;
    private bool canPlaceItemAtCursorPosition;
    private bool cursorIsOverItemWorld;
    private bool placingItem;

    private void Awake()
    {
        WaitingForInputReleaseBeforePlacement = false;

        playerInventory = player.GetInventory();

        itemPickupAndPlacementAction = itemPickupAndPlacementActionReference.action;
    }

    private void Start()
    {
        DefaultState = new DefaultItemPickupAndPlacementState(this);
        PickupState = new ItemPickupState(this, itemPickupAndPlacementAction);
        PlacementState = new ItemPlacementState(this, itemPickupAndPlacementAction);

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

        UpdateInstanceVariables();

        currentState.StateUpdate();
    }

    public void PickUpHoveredItem()
    {
        ItemWithAmount hoveredItem = hoveredItemWorld.GetItem();

        bool canAddItem =
            playerInventory.CanAddItem(hoveredItem, out int canAddAmount);

        if (canAddItem)
        {
            playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(hoveredItem,
                itemSelectionController.SelectedItemIndex);

            Destroy(hoveredItemWorld.gameObject);
        }
        else if (canAddAmount != 0)
        {
            ItemWithAmount itemToAdd = new ItemWithAmount(hoveredItem.itemData,
                canAddAmount, hoveredItem.instanceProperties);

            playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(itemToAdd,
                itemSelectionController.SelectedItemIndex);

            hoveredItemWorld.SetItemAmount(hoveredItem.amount - canAddAmount);
        }
        else
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();

            return;
        }
    }

    public void PlaceSelectedPlayerHotbarItemAtCursorPosition()
    {
        ItemWithAmount itemToPlace = player.GetSelectedItem();
        int itemToPlaceIndex = player.GetSelectedItemIndex();

        if (itemToPlace.itemData.name != "Empty")
        {
            _ =ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(cursorPoint, itemToPlace);
            playerInventory.RemoveItemAtIndex(itemToPlaceIndex);
        }
    }

    public void ResetCursor()
    {
        cursorController.UseDefaultCursor();
    }

    public void ResetCursorAndPlayerItem()
    {
        ResetCursor();
        playerItemInWorld.HideCharacterItemInWorld();
    }

    public void UsePickupCursor()
    {
        if (hoveredItemWorld != null &&
            !playerInventory.CanAddItem(hoveredItemWorld.GetItem(), out int canAddAmount) &&
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
        ItemWithAmount selectedItem = player.GetSelectedItem();

        Color cursorBackgroundColor = canPlaceItemAtCursorPosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItem.itemData);

        cursorController.UpdateUsingItem(selectedItem);
        cursorController.UpdateCursorBackground(cursorBackgroundColor,
            selectedItemColliderSize);

        playerItemInWorld.ShowCharacterItemInWorld(selectedItem);
    }

    private void UpdateInstanceVariables()
    {
        cursorPoint = cursorController.GetWorldPosition();

        Collider2D cursorOverlapCollider = Physics2D.OverlapPoint(cursorPoint);

        bool cursorIsOverCollider = cursorOverlapCollider != null;
        cursorIsOverItemWorld = cursorIsOverCollider && cursorOverlapCollider.CompareTag("Item World");

        if (cursorIsOverItemWorld)
        {
            hoveredItemWorld = cursorOverlapCollider.GetComponent<ItemWorld>();
        }
        else
        {
            hoveredItemWorld = null;
        }

        ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

        Vector2 selectedItemColliderExtents =
            ItemWorldPrefabInstanceFactory.GetItemColliderExtents(selectedItemData);

        canPlaceItemAtCursorPosition =
             SpawnColliderHelper.CanSpawnColliderAtPosition(cursorPoint, selectedItemColliderExtents);

        placingItem = itemPickupAndPlacementAction.IsPressed() &&
            !WaitingForInputReleaseBeforePlacement && selectedItemData.name != "Empty";
    }

    public void SwitchState(BaseItemPickupAndPlacementState newState)
    {
        currentState.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }

    public bool CanPlaceItemAtCursorPosition() => canPlaceItemAtCursorPosition;

    public bool CursorIsOverItemWorld() => cursorIsOverItemWorld;

    public bool PlacingItem() => placingItem;

    public ItemWorld GetHoveredItemWorld() => hoveredItemWorld;
}
