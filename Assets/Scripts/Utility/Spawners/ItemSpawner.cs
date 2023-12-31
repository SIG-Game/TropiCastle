using UnityEngine;

public class ItemSpawner : PrefabSpawner
{
    [SerializeField] private ItemStack itemToSpawn;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        ItemWorld spawnedItemWorld = spawnedPrefab.GetComponent<ItemWorld>();

        spawnedItemWorld.Item = itemToSpawn;
    }
}
