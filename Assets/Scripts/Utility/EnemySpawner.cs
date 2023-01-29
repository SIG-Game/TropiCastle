using UnityEngine;

public class EnemySpawner : PrefabSpawner
{
    [SerializeField] private Transform player;

    protected override void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        Enemy spawnedEnemy = spawnedPrefab.GetComponent<Enemy>();

        spawnedEnemy.SetPlayerTransform(player);
        spawnedEnemy.SetSpawner(this);

        Debug.Log($"Spawned enemy {prefabToSpawn.gameObject.name} at {spawnedPrefab.transform.position}.");
    }
}
