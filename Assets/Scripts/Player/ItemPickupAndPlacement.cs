using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private PlayerItemInWorldController playerItemInWorld;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;

    public bool WaitingForInputReleaseBeforePlacement { get; set; }

    public DefaultItemPickupAndPlacementState DefaultState { get; private set; }
    public ItemPickupState PickupState { get; private set; }
    public ItemPlacementState PlacementState { get; private set; }

    private BaseItemPickupAndPlacementState currentState;
    private Inventory playerInventory;
    private Vector2 mouseWorldPoint;
    private ItemWorld hoveredItemWorld;
    private InputAction itemPickupAndPlacementAction;
    private bool canPlaceItemAtMousePosition;
    private bool mouseIsOverItemWorld;
    private bool placingItem;

    private void Awake()
    {
        WaitingForInputReleaseBeforePlacement = false;
    }

    private void Start()
    {
        playerInventory = player.GetInventory();

        itemPickupAndPlacementAction = InputManager.Instance.GetAction("Item Pickup and Placement");

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

            if (PickupState.ResetCursorOnPickupInputRelease)
            {
                ResetCursor();

                PickupState.ResetCursorOnPickupInputRelease = false;
            }
        }

        if (PauseController.Instance.GamePaused || PlayerController.ActionDisablingUIOpen ||
            player.IsAttacking)
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

        return;
    }

    public void PickUpHoveredItem()
    {
        if (playerInventory.IsFull())
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();

            return;
        }

        playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(hoveredItemWorld.item,
            itemSelectionController.SelectedItemIndex);
        Destroy(hoveredItemWorld.gameObject);
    }

    public void PlaceSelectedPlayerHotbarItemAtMousePosition()
    {
        ItemWithAmount itemToPlace = player.GetSelectedItem();
        int itemToPlaceIndex = player.GetSelectedItemIndex();

        if (itemToPlace.itemData.name != "Empty")
        {
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(mouseWorldPoint, itemToPlace);
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
        playerItemInWorld.HidePlayerItemInWorld();
    }

    public void UsePickupCursor()
    {
        cursorController.SetCursorSprite(itemPickupArrow);
        cursorController.SetCursorBackgroundColor(Color.clear);
    }

    public void UsePlacementCursorAndPlayerItem()
    {
        ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItemData);

        cursorController.SetCursorBackgroundLocalScale(selectedItemColliderSize);

        cursorController.SetCursorSprite(selectedItemData.sprite);
        cursorController.SetCursorBackgroundColor(canPlaceItemAtMousePosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor);

        playerItemInWorld.ShowPlayerItemInWorld(selectedItemData.sprite);
    }

    private void UpdateInstanceVariables()
    {
        mouseWorldPoint = Camera.main.ScreenToWorldPoint(
            MousePositionHelper.GetMousePositionClampedToScreen());

        Collider2D mouseOverlapCollider = Physics2D.OverlapPoint(mouseWorldPoint);

        bool mouseIsOverCollider = mouseOverlapCollider != null;
        mouseIsOverItemWorld = mouseIsOverCollider && mouseOverlapCollider.CompareTag("Item World");

        if (mouseIsOverItemWorld)
        {
            hoveredItemWorld = mouseOverlapCollider.GetComponent<ItemWorld>();
        }
        else
        {
            hoveredItemWorld = null;
        }

        ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

        Vector2 selectedItemColliderExtents =
            ItemWorldPrefabInstanceFactory.GetItemColliderExtents(selectedItemData);

        canPlaceItemAtMousePosition =
             SpawnColliderHelper.CanSpawnColliderAtPosition(mouseWorldPoint, selectedItemColliderExtents);

        placingItem = itemPickupAndPlacementAction.IsPressed() &&
            !WaitingForInputReleaseBeforePlacement && selectedItemData.name != "Empty";
    }

    public void SwitchState(BaseItemPickupAndPlacementState newState)
    {
        currentState.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }

    public bool CanPlaceItemAtMousePosition() => canPlaceItemAtMousePosition;

    public bool MouseIsOverItemWorld() => mouseIsOverItemWorld;

    public bool PlacingItem() => placingItem;
}
