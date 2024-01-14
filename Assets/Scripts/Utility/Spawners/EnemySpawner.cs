using UnityEngine;

public class EnemySpawner : PrefabSpawner
{
    [SerializeField] private Inventory playerInventory;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        EnemyController spawnedEnemy = spawnedPrefab.GetComponent<EnemyController>();

        spawnedEnemy.SetPlayerInventory(playerInventory);
    }
}
