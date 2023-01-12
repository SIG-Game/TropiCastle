using UnityEngine;

public class ItemPlacementTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private bool canPlace;

    private void Awake()
    {
        canPlace = true;
    }

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1) && canPlace)
        {
            ItemWithAmount itemToPlace = player.GetHotbarItem();

            if (itemToPlace.itemData.name != "Empty")
            {
                Vector3 itemPosition = transform.position;
                itemPosition.z = 0f;

                _ = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(itemPosition, itemToPlace);
                player.GetInventory().RemoveItem(itemToPlace);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canPlace = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPlace = true;
    }
}
