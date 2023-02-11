using UnityEngine;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;
    [SerializeField] private GameObject pickupArrow;

    private Inventory playerInventory;
    private Vector2 itemWorldPrefabColliderExtents;
    private SpriteRenderer pickupArrowSpriteRenderer;

    private void Start()
    {
        playerInventory = player.GetInventory();
        itemWorldPrefabColliderExtents = ItemWorldPrefabInstanceFactory.Instance.GetItemWorldPrefabColliderExtents();
        pickupArrowSpriteRenderer = pickupArrow.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            if (pickupArrowSpriteRenderer.color != Color.clear)
            {
                pickupArrowSpriteRenderer.color = Color.clear;
            }

            return;
        }

        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D mouseOverlapCollider = Physics2D.OverlapPoint(mouseWorldPoint);

        pickupArrow.transform.position = mouseWorldPoint;

        SetPickupArrowColorFromCollider(mouseOverlapCollider);

        if (Input.GetMouseButtonDown(1))
        {
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

    private void SetPickupArrowColorFromCollider(Collider2D collider)
    {
        if (collider != null && collider.GetComponent<ItemWorld>() != null)
        {
            pickupArrowSpriteRenderer.color = Color.white;
        }
        else
        {
            pickupArrowSpriteRenderer.color = Color.clear;
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
