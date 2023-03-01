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

    private void Start()
    {
        playerInventory = player.GetInventory();
        itemWorldPrefabColliderExtents = ItemWorldPrefabInstanceFactory.Instance.GetItemWorldPrefabColliderExtents();

        cursorController.SetCursorBackgroundLocalScale(itemWorldPrefabColliderExtents * 2f);
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
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
        bool placingItem = Input.GetMouseButton(1) && player.GetSelectedItem().itemData.name != "Empty";

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

        if (Input.GetMouseButtonUp(1))
        {
            if (mouseIsOverItemWorld)
            {
                PickUpItemFromItemWorld(mouseOverlapCollider.GetComponent<ItemWorld>());
            }
            else if (CanPlaceItemAtPosition(mouseWorldPoint) && mouseIsOnScreen)
            {
                PlaceSelectedPlayerHotbarItemAtPosition(mouseWorldPoint);
            }

            cursorController.UseDefaultCursor();
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
