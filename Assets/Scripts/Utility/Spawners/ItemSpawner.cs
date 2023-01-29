using UnityEngine;

public class ItemSpawner : PrefabSpawner
{
    [SerializeField] private ItemWithAmount itemToSpawn;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        spawnedPrefab.name = $"{itemToSpawn.itemData.name} ItemWorld";

        ItemWorld spawnedItemWorld = spawnedPrefab.GetComponent<ItemWorld>();

        spawnedItemWorld.item = itemToSpawn;
        spawnedItemWorld.spawner = this;
    }
}
