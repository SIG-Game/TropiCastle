using UnityEngine;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;
    [SerializeField] private GameObject cursorIcon;
    [SerializeField] private GameObject cursorIconBackground;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Color canPlaceCursorIconBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorIconBackgroundColor;

    private Inventory playerInventory;
    private Vector2 itemWorldPrefabColliderExtents;
    private SpriteRenderer cursorIconSpriteRenderer;
    private SpriteRenderer cursorIconBackgroundSpriteRenderer;

    private void Start()
    {
        playerInventory = player.GetInventory();
        itemWorldPrefabColliderExtents = ItemWorldPrefabInstanceFactory.Instance.GetItemWorldPrefabColliderExtents();

        cursorIconSpriteRenderer = cursorIcon.GetComponent<SpriteRenderer>();
        cursorIconBackgroundSpriteRenderer = cursorIconBackground.GetComponent<SpriteRenderer>();

        cursorIconBackground.transform.localScale = itemWorldPrefabColliderExtents * 2f;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            HideCursorIcon();
            return;
        }

        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D mouseOverlapCollider = Physics2D.OverlapPoint(mouseWorldPoint);

        cursorIcon.transform.position = mouseWorldPoint;

        bool mouseIsOverCollider = mouseOverlapCollider != null;
        bool mouseIsOverItemWorld = mouseIsOverCollider && mouseOverlapCollider.CompareTag("Item World");
        bool placingItem = Input.GetMouseButton(1) && player.GetSelectedItem().itemData.name != "Empty";

        if (mouseIsOverItemWorld)
        {
            cursorIconSpriteRenderer.sprite = itemPickupArrow;
            cursorIconBackgroundSpriteRenderer.color = Color.clear;
        }
        else if (placingItem)
        {
            cursorIconSpriteRenderer.sprite = player.GetSelectedItem().itemData.sprite;
            cursorIconBackgroundSpriteRenderer.color = CanPlaceItemAtPosition(mouseWorldPoint) ?
                canPlaceCursorIconBackgroundColor : cannotPlaceCursorIconBackgroundColor;
        }
        else
        {
            HideCursorIcon();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (mouseIsOverItemWorld)
            {
                PickUpItemFromItemWorld(mouseOverlapCollider.GetComponent<ItemWorld>());
            }
            else if (CanPlaceItemAtPosition(mouseWorldPoint))
            {
                PlaceSelectedPlayerHotbarItemAtPosition(mouseWorldPoint);
            }

            HideCursorIcon();
        }
    }

    private void PickUpItemFromItemWorld(ItemWorld itemWorld)
    {
        if (playerInventory.IsFull())
        {
            inventoryFullUIController.ShowInventoryFullText();
            return;
        }

        playerInventory.AddItem(itemWorld.item);
        Destroy(itemWorld.gameObject);
    }

    private void PlaceSelectedPlayerHotbarItemAtPosition(Vector2 position)
    {
        ItemWithAmount itemToPlace = player.GetSelectedItem();

        if (itemToPlace.itemData.name != "Empty")
        {
            _ = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(position, itemToPlace);
            playerInventory.RemoveItem(itemToPlace);
        }
    }

    private bool CanPlaceItemAtPosition(Vector2 position)
    {
        Vector2 overlapAreaCornerBottomLeft = position - itemWorldPrefabColliderExtents;
        Vector2 overlapAreaCornerTopRight = position + itemWorldPrefabColliderExtents;

        Collider2D itemWorldOverlap = Physics2D.OverlapArea(overlapAreaCornerBottomLeft, overlapAreaCornerTopRight);

        return itemWorldOverlap == null;
    }

    private void HideCursorIcon()
    {
        cursorIconSpriteRenderer.sprite = null;
        cursorIconBackgroundSpriteRenderer.color = Color.clear;
    }
}
