using UnityEngine;

public class ItemPlacementTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            // 0.2f is half the size of the Item World prefab's hitbox
            Vector2 overlapAreaCornerA = new Vector2(transform.position.x - 0.2f, transform.position.y - 0.2f);
            Vector2 overlapAreaCornerB = new Vector2(transform.position.x + 0.2f, transform.position.y + 0.2f);
            Collider2D itemWorldOverlap = Physics2D.OverlapArea(overlapAreaCornerA, overlapAreaCornerB);

            if (itemWorldOverlap == null)
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
    }
}
