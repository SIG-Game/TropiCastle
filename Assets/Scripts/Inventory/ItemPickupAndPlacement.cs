using UnityEngine;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;

    private Inventory playerInventory;

    private void Start()
    {
        playerInventory = player.GetInventory();
    }

    private void Update()
    {
        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            Collider2D mouseOverlapPoint = Physics2D.OverlapPoint(mouseWorldPoint);

            // Pick up item
            if (mouseOverlapPoint != null)
            {
                ItemWorld itemWorld = mouseOverlapPoint.GetComponent<ItemWorld>();

                if (itemWorld != null)
                {
                    if (playerInventory.IsFull())
                    {
                        inventoryFullUIController.ShowInventoryFullText();
                        return;
                    }

                    if (itemWorld.spawner != null)
                    {
                        itemWorld.spawner.SpawnedPrefabRemoved();
                    }

                    playerInventory.AddItem(itemWorld.item);
                    Destroy(mouseOverlapPoint.gameObject);
                }
            }
            // Attempt to place item
            else
            {
                // 0.2f is half the size of the Item World prefab's hitbox
                Vector2 overlapAreaCornerA = new Vector2(mouseWorldPoint.x - 0.2f, mouseWorldPoint.y - 0.2f);
                Vector2 overlapAreaCornerB = new Vector2(mouseWorldPoint.x + 0.2f, mouseWorldPoint.y + 0.2f);
                Collider2D itemWorldOverlap = Physics2D.OverlapArea(overlapAreaCornerA, overlapAreaCornerB);

                if (itemWorldOverlap == null)
                {
                    ItemWithAmount itemToPlace = player.GetHotbarItem();

                    if (itemToPlace.itemData.name != "Empty")
                    {
                        Vector3 itemPosition = mouseWorldPoint;
                        itemPosition.z = 0f;

                        _ = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(itemPosition, itemToPlace);
                        playerInventory.RemoveItem(itemToPlace);
                    }
                }
            }
        }
    }
}
