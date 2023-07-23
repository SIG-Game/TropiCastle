using UnityEngine;

public class ItemSpawner : PrefabSpawner
{
    [SerializeField] private ItemWithAmount itemToSpawn;
    [SerializeField] private ItemInteractableDependencies itemInteractableDependencies;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        ItemWorld spawnedItemWorld = spawnedPrefab.GetComponent<ItemWorld>();

        spawnedItemWorld.SetUpItemWorld(itemToSpawn, itemInteractableDependencies);
    }
}
