using UnityEngine;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;

    private Inventory playerInventory;
    private Vector2 itemWorldPrefabColliderExtents;

    private void Start()
    {
        playerInventory = player.GetInventory();
        itemWorldPrefabColliderExtents = ItemWorldPrefabInstanceFactory.Instance.GetItemWorldPrefabColliderExtents();
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D mouseOverlapCollider = Physics2D.OverlapPoint(mouseWorldPoint);

            bool mouseIsOverCollider = mouseOverlapCollider != null;
            if (mouseIsOverCollider)
            {
                AttemptToPickUpItemFromCollider(mouseOverlapCollider);
            }
            else if (CanPlaceItemAtPosition(mouseWorldPoint))
            {
                PlaceSelectedPlayerHotbarItemAtPosition(mouseWorldPoint);
            }
        }
    }

    private void AttemptToPickUpItemFromCollider(Collider2D collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();

        if (itemWorld == null)
        {
            return;
        }

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
}
