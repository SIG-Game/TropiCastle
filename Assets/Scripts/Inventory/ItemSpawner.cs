using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemWithAmount itemToSpawn;
    [SerializeField] private float spawnRateSeconds;

    private bool itemSpawned;

    private const float minX = -6f;
    private const float maxX = 6f;
    private const float minY = -2f;
    private const float maxY = 4f;

    private void Start()
    {
        itemSpawned = false;

        InvokeRepeating("SpawnItem", 0f, spawnRateSeconds);
    }

    private void SpawnItem()
    {
        if (!itemSpawned)
        {
            Vector2 spawnLocation = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            ItemWorld spawnedItemWorld = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(spawnLocation, itemToSpawn);

            spawnedItemWorld.spawner = this;

            itemSpawned = true;

            Debug.Log($"Spawned item {spawnedItemWorld.item.itemData.name} at {spawnLocation}.");
        }
    }

    public void SpawnedItemWorldPrefabInstanceRemoved()
    {
        itemSpawned = false;
    }
}
