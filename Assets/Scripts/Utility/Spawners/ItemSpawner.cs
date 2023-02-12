using UnityEngine;

public class ItemSpawner : PrefabSpawner
{
    [SerializeField] private ItemWithAmount itemToSpawn;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        ItemWorld spawnedItemWorld = spawnedPrefab.GetComponent<ItemWorld>();

        spawnedItemWorld.item = itemToSpawn;
    }
}
