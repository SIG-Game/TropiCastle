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
    private bool waitingForRightClickReleaseBeforePlacement;

    private void Start()
    {
        playerInventory = player.GetInventory();
        waitingForRightClickReleaseBeforePlacement = false;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused || PlayerController.ActionDisablingUIOpen)
        {
            // This check needs to occur in case right-click is released while the
            // above condition is true after picking up an item on press of that click
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
            ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

            Vector2 selectedItemColliderSize =
                ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItemData);

            cursorController.SetCursorBackgroundLocalScale(selectedItemColliderSize);

            cursorController.SetCursorSprite(selectedItemData.sprite);
            cursorController.SetCursorBackgroundColor(CanPlaceItemAtPosition(mouseWorldPoint) ?
                canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor);
        }
        else
        {
            cursorController.UseDefaultCursor();
        }

        if (Input.GetMouseButtonDown(1) && mouseIsOverItemWorld)
        {
            if (TryPickUpItemFromItemWorld(mouseOverlapCollider.GetComponent<ItemWorld>()))
            {
                cursorController.UseDefaultCursor();
            }

            // Prevent item placement on right-click release from using
            // the same click as item pickup on right-click press
            waitingForRightClickReleaseBeforePlacement = true;
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
                if (TryPickUpItemFromItemWorld(mouseOverlapCollider.GetComponent<ItemWorld>()))
                {
                    cursorController.UseDefaultCursor();
                }
            }
            else if (CanPlaceItemAtPosition(mouseWorldPoint) && mouseIsOnScreen &&
                !waitingForRightClickReleaseBeforePlacement)
            {
                PlaceSelectedPlayerHotbarItemAtPosition(mouseWorldPoint);
                cursorController.UseDefaultCursor();
            }

            waitingForRightClickReleaseBeforePlacement = false;
        }
    }

    private bool TryPickUpItemFromItemWorld(ItemWorld itemWorld)
    {
        if (playerInventory.IsFull())
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
            return false;
        }

        playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(itemWorld.item,
            itemSelectionController.SelectedItemIndex);
        Destroy(itemWorld.gameObject);
        return true;
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
        Vector2 selectedItemColliderExtents =
            ItemWorldPrefabInstanceFactory.GetItemColliderExtents(player.GetSelectedItem().itemData);

        return SpawnColliderHelper.CanSpawnColliderAtPosition(position, selectedItemColliderExtents);
    }
}
