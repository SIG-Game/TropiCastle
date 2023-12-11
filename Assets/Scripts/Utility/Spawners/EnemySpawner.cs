using UnityEngine;

public class EnemySpawner : PrefabSpawner
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemWorldPrefabInstanceFactory itemWorldPrefabInstanceFactory;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        EnemyController spawnedEnemy = spawnedPrefab.GetComponent<EnemyController>();

        spawnedEnemy.SetUpEnemy(playerTransform, playerInventory,
            itemWorldPrefabInstanceFactory);
    }
}
