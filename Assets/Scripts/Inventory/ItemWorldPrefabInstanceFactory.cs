using UnityEngine;

public class ItemWorldPrefabInstanceFactory : MonoBehaviour
{
    [SerializeField] private GameObject itemWorldPrefab;
    [SerializeField] private Transform itemWorldParent;

    private Vector2 itemWorldPrefabColliderExtents;

    private const int maxDropSpawnAttempts = 20;

    public static ItemWorldPrefabInstanceFactory Instance;

    private void Awake()
    {
        Instance = this;

        // BoxCollider2D bounds property (which has extents property) is not set
        // until prefab is instantiated, so that property cannot be used here
        itemWorldPrefabColliderExtents = itemWorldPrefab.GetComponent<BoxCollider2D>().size / 2f;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SpawnItemWorld(Vector3 position, ItemWithAmount itemToSpawn)
    {
        GameObject spawnedGameObject = Instantiate(itemWorldPrefab, position, Quaternion.identity, itemWorldParent);
        ItemWorld spawnedItemWorld = spawnedGameObject.GetComponent<ItemWorld>();

        spawnedItemWorld.item = itemToSpawn;
    }

    public void DropItem(Vector3 dropPosition, ItemWithAmount itemToDrop)
    {
        if (itemToDrop.itemData.name == "Empty")
            return;

        Vector2 spawnPosition;
        bool canSpawnItemToDrop;
        int spawnAttempts = 0;

        do
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            randomOffset.Normalize();
            randomOffset *= 0.7f;

            spawnPosition = dropPosition + randomOffset;

            canSpawnItemToDrop = CanSpawnItemWorldAtPosition(spawnPosition);

            spawnAttempts++;

            if (spawnAttempts == maxDropSpawnAttempts && !canSpawnItemToDrop)
            {
                Debug.LogWarning($"Failed to drop {itemToDrop.itemData.name} item outside of " +
                    $"colliders after {maxDropSpawnAttempts} attempts");
                return;
            }
        } while (!canSpawnItemToDrop);

        SpawnItemWorld(spawnPosition, itemToDrop);

        // Dropped items currently don't move
        //itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }

    private bool CanSpawnItemWorldAtPosition(Vector2 position)
    {
        Vector2 overlapAreaCornerBottomLeft = position - itemWorldPrefabColliderExtents;
        Vector2 overlapAreaCornerTopRight = position + itemWorldPrefabColliderExtents;

        Collider2D itemWorldOverlap = Physics2D.OverlapArea(overlapAreaCornerBottomLeft, overlapAreaCornerTopRight);

        return itemWorldOverlap == null;
    }

    public Vector2 GetItemWorldPrefabColliderExtents() => itemWorldPrefabColliderExtents;
}
