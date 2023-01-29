using UnityEngine;

// TODO: Make enemy spawner and item spawner subclasses of this class
public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private ItemWithAmount itemToSpawn;
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

        if (itemToSpawn.itemData != null)
        {
            ItemWorld spawnedItemWorld = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(spawnPosition, itemToSpawn);
            spawnedItemWorld.spawner = this;

            Debug.Log($"Spawned item {spawnedItemWorld.item.itemData.name} at {spawnPosition}.");
        }
        else
        {
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            Enemy spawnedEnemy = spawnedPrefab.GetComponent<Enemy>();

            if (spawnedEnemy != null) {
                spawnedEnemy.SetPlayerTransform(player);
                spawnedEnemy.SetSpawner(this);
            }

            Debug.Log($"Spawned prefab {prefabToSpawn.gameObject.name} at {spawnPosition}.");
        }

        numPrefabs++;
    }

    public void SpawnedPrefabRemoved()
    {
        numPrefabs--;
    }
}
