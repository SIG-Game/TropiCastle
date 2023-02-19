using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private int maxSpawnedPrefabs;
    [SerializeField] private Vector2 minSpawnPosition;
    [SerializeField] private Vector2 maxSpawnPosition;
    [SerializeField] private bool logOnSpawn;
    [SerializeField] private bool drawSpawnArea;

    private int numPrefabs;
    private float spawnTimer;
    private Vector3 topLeftCornerSpawnArea;
    private Vector3 topRightCornerSpawnArea;
    private Vector3 bottomLeftCornerSpawnArea;
    private Vector3 bottomRightCornerSpawnArea;

    private void Awake()
    {
        if (prefabToSpawn.GetComponent<Spawnable>() == null)
        {
            Debug.LogError($"{nameof(prefabToSpawn)} named {prefabToSpawn.gameObject.name} " +
                $"must have {nameof(Spawnable)} component. Destroying spawner...");
            Destroy(gameObject);
        }

        topLeftCornerSpawnArea = new Vector3(minSpawnPosition.x, maxSpawnPosition.y);
        topRightCornerSpawnArea = new Vector3(maxSpawnPosition.x, maxSpawnPosition.y);
        bottomLeftCornerSpawnArea = new Vector3(minSpawnPosition.x, minSpawnPosition.y);
        bottomRightCornerSpawnArea = new Vector3(maxSpawnPosition.x, minSpawnPosition.y);
    }

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
        if (drawSpawnArea)
        {
            Debug.DrawLine(topLeftCornerSpawnArea, topRightCornerSpawnArea);
            Debug.DrawLine(topRightCornerSpawnArea, bottomRightCornerSpawnArea);
            Debug.DrawLine(bottomRightCornerSpawnArea, bottomLeftCornerSpawnArea);
            Debug.DrawLine(bottomLeftCornerSpawnArea, topLeftCornerSpawnArea);
        }

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

        GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
        spawnedPrefab.GetComponent<Spawnable>().SetSpawner(this);

        ApplySpawnedPrefabProperties(spawnedPrefab);

        if (logOnSpawn)
        {
            Debug.Log($"Spawned {spawnedPrefab.gameObject.name} at {spawnedPrefab.transform.position}.");
        }

        numPrefabs++;
    }

    protected virtual void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
    }

    public void SpawnedPrefabDestroyed()
    {
        numPrefabs--;
    }
}
