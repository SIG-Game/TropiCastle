using UnityEngine;

public class EnemySpawner : PrefabSpawner
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        Enemy spawnedEnemy = spawnedPrefab.GetComponent<Enemy>();

        spawnedEnemy.SetPlayerTransform(playerTransform);
        spawnedEnemy.SetPlayerInventory(playerInventory);
    }
}
