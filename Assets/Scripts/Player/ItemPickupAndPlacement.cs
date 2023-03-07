using UnityEngine;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;

    private Inventory playerInventory;
    private Vector2 itemWorldPrefabColliderExtents;
    private bool waitingForRightClickReleaseBeforePlacement;

    private void Start()
    {
        playerInventory = player.GetInventory();
        itemWorldPrefabColliderExtents = ItemWorldPrefabInstanceFactory.Instance.GetItemWorldPrefabColliderExtents();
        waitingForRightClickReleaseBeforePlacement = false;

        cursorController.SetCursorBackgroundLocalScale(itemWorldPrefabColliderExtents * 2f);
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            // This check needs to occur in case right-click is released while
            // the game is paused after picking up an item on press of that click
            if (Input.GetMouseButtonUp(1))
            {
                waitingForRightClickReleaseBeforePlacement = false;
            }

            return;
        }

        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D mouseOverlapCollider = Physics2D.OverlapPoint(mouseWorldPoint);

        float mouseXPosition = Input.mousePosition.x;
        float mouseYPosition = Input.mousePosition.y;
        bool mouseIsOnScreen = mouseXPosition >= 0f && mouseXPosition <= Screen.width &&
            mouseYPosition >= 0f && mouseYPosition <= Screen.height;

        bool mouseIsOverCollider = mouseOverlapCollider != null;
        bool mouseIsOverItemWorld = mouseIsOverCollider && mouseOverlapCollider.CompareTag("Item World");
        bool placingItem = Input.GetMouseButton(1) && !waitingForRightClickReleaseBeforePlacement
            && player.GetSelectedItem().itemData.name != "Empty";

        if (mouseIsOverItemWorld)
        {
            cursorController.SetCursorSprite(itemPickupArrow);
            cursorController.SetCursorBackgroundColor(Color.clear);
        }
        else if (placingItem && mouseIsOnScreen)
        {
            cursorController.SetCursorSprite(player.GetSelectedItem().itemData.sprite);
            cursorController.SetCursorBackgroundColor(CanPlaceItemAtPosition(mouseWorldPoint) ?
                canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor);
        }
        else
        {
            cursorController.UseDefaultCursor();
        }

        if (Input.GetMouseButtonDown(1) && mouseIsOverItemWorld)
        {
            PickUpItemFromItemWorld(mouseOverlapCollider.GetComponent<ItemWorld>());

            // Prevent item placement on right-click release from using
            // the same click as item pickup on right-click press
            waitingForRightClickReleaseBeforePlacement = true;

            cursorController.UseDefaultCursor();
        }

        // Cancel item placement when left-click is pressed while placing an item
        if (placingItem && InputManager.Instance.GetLeftClickDownIfUnusedThisFrame())
        {
            waitingForRightClickReleaseBeforePlacement = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (mouseIsOverItemWorld)
            {
                PickUpItemFromItemWorld(mouseOverlapCollider.GetComponent<ItemWorld>());
            }
            else if (CanPlaceItemAtPosition(mouseWorldPoint) && mouseIsOnScreen &&
                !waitingForRightClickReleaseBeforePlacement)
            {
                PlaceSelectedPlayerHotbarItemAtPosition(mouseWorldPoint);
            }

            cursorController.UseDefaultCursor();

            waitingForRightClickReleaseBeforePlacement = false;
        }
    }

    private void PickUpItemFromItemWorld(ItemWorld itemWorld)
    {
        if (playerInventory.IsFull())
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
            return;
        }

        playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(itemWorld.item,
            itemSelectionController.SelectedItemIndex);
        Destroy(itemWorld.gameObject);
    }

    private void PlaceSelectedPlayerHotbarItemAtPosition(Vector2 position)
    {
        ItemWithAmount itemToPlace = player.GetSelectedItem();
        int itemToPlaceIndex = player.GetSelectedItemIndex();

        if (itemToPlace.itemData.name != "Empty")
        {
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(position, itemToPlace);
            playerInventory.RemoveItemAtIndex(itemToPlaceIndex);
        }
    }

    private bool CanPlaceItemAtPosition(Vector2 position)
    {
        Vector2 overlapAreaCornerBottomLeft = position - itemWorldPrefabColliderExtents;
        Vector2 overlapAreaCornerTopRight = position + itemWorldPrefabColliderExtents;

        Collider2D itemWorldOverlap = Physics2D.OverlapArea(overlapAreaCornerBottomLeft, overlapAreaCornerTopRight);

        return itemWorldOverlap == null;
    }
}
