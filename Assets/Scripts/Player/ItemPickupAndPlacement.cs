using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private PlayerItemInWorldController playerItemInWorld;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Sprite itemPickupArrowInventoryFull;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;

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

        playerInventory.OnItemAdded += PlayerInventory_OnItemAdded;
        playerInventory.OnItemRemoved += PlayerInventory_OnItemRemoved;
    }

    private void Start()
    {
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
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded -= PlayerInventory_OnItemAdded;
            playerInventory.OnItemRemoved -= PlayerInventory_OnItemRemoved;
        }
    }

    public void PickUpHoveredItem()
    {
        if (playerInventory.HasNoEmptySlots())
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();

            return;
        }

        playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(hoveredItemWorld.Item,
            itemSelectionController.SelectedItemIndex);
        Destroy(hoveredItemWorld.gameObject);
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
        playerItemInWorld.HidePlayerItemInWorld();
    }

    public void UsePickupCursor()
    {
        if (playerInventory.HasNoEmptySlots())
        {
            cursorController.Sprite = itemPickupArrowInventoryFull;
        }
        else
        {
            cursorController.Sprite = itemPickupArrow;
        }

        cursorController.SetCursorBackgroundColor(Color.clear);
    }

    public void UsePlacementCursorAndPlayerItem()
    {
        ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItemData);

        cursorController.SetCursorBackgroundLocalScale(selectedItemColliderSize);

        cursorController.Sprite = selectedItemData.sprite;
        cursorController.SetCursorBackgroundColor(canPlaceItemAtCursorPosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor);

        playerItemInWorld.ShowPlayerItemInWorld(selectedItemData.sprite);
    }

    private void UpdateInstanceVariables()
    {
        cursorPoint = cursorController.GetPosition();

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

    private void PlayerInventory_OnItemAdded(ItemWithAmount _)
    {
        if (playerInventory.HasNoEmptySlots() &&
            cursorController.Sprite == itemPickupArrow)
        {
            cursorController.Sprite = itemPickupArrowInventoryFull;
        }
    }

    private void PlayerInventory_OnItemRemoved(ItemWithAmount _)
    {
        if (cursorController.Sprite == itemPickupArrowInventoryFull)
        {
            cursorController.Sprite = itemPickupArrow;
        }
    }

    public bool CanPlaceItemAtCursorPosition() => canPlaceItemAtCursorPosition;

    public bool CursorIsOverItemWorld() => cursorIsOverItemWorld;

    public bool PlacingItem() => placingItem;
}
