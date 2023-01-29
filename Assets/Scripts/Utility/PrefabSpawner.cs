using UnityEngine;

// TODO: Make enemy spawner subclass of this class
public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] private Transform player;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private int maxSpawnedPrefabs;
    [SerializeField] private Vector2 minSpawnPosition;
    [SerializeField] private Vector2 maxSpawnPosition;

    private int numPrefabs;
    private float spawnTimer;

    private void Start()
    {
        numPrefabs = 0;
        spawnTimer = 0f;

        if (numPrefabs < maxSpawnedPrefabs)
        {
            SpawnPrefab();
        }
    }

    private void Update()
    {
        if (numPrefabs >= maxSpawnedPrefabs)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= timeBetweenSpawns)
        {
            spawnTimer = 0f;
            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        float spawnXPosition = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
        float spawnYPosition = Random.Range(minSpawnPosition.y, maxSpawnPosition.y);
        Vector2 spawnPosition = new Vector2(spawnXPosition, spawnYPosition);

        GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        ApplySpawnedPrefabProperties(spawnedPrefab);

        numPrefabs++;
    }

    protected virtual void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
        Enemy spawnedEnemy = spawnedPrefab.GetComponent<Enemy>();

        if (spawnedEnemy != null)
        {
            spawnedEnemy.SetPlayerTransform(player);
            spawnedEnemy.SetSpawner(this);
        }

        Debug.Log($"Spawned prefab {prefabToSpawn.gameObject.name} at {spawnedPrefab.transform.position}.");
    }

    public void SpawnedPrefabRemoved()
    {
        numPrefabs--;
    }
}
