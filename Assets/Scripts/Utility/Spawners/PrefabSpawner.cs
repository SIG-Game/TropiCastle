using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] private SpawnerSaveManager spawnerSaveManager;
    [SerializeField] private Vector2 timeBeforeFirstSpawnRange;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private int maxSpawnedPrefabs;
    [SerializeField] private Vector2 minSpawnPosition;
    [SerializeField] private Vector2 maxSpawnPosition;

    public int NumPrefabs { get; set; }
    public float SpawnTimer { get; set; }
    public bool WaitBeforeFirstSpawnCompleted { get; set; }

    private BoxCollider2D prefabToSpawnBoxCollider;
    private Vector2 prefabToSpawnColliderExtents;
    private WaitForSeconds beforeFirstSpawnWaitForSeconds;

    private const int maxSpawnAttempts = 20;

    private void Awake()
    {
        if (prefabToSpawn.GetComponent<Spawnable>() == null)
        {
            Debug.LogError($"{nameof(prefabToSpawn)} named {prefabToSpawn.name} " +
                $"must have {nameof(Spawnable)} component. Destroying spawner...");
            Destroy(gameObject);
        }

        prefabToSpawnBoxCollider = prefabToSpawn.GetComponent<BoxCollider2D>();
        prefabToSpawnColliderExtents = prefabToSpawnBoxCollider.size / 2f;
    }

    private void Start()
    {
        if (!WaitBeforeFirstSpawnCompleted)
        {
            float beforeFirstSpawnWait = Random.Range(timeBeforeFirstSpawnRange.x,
            timeBeforeFirstSpawnRange.y);

            beforeFirstSpawnWaitForSeconds = new WaitForSeconds(beforeFirstSpawnWait);

            StartCoroutine(WaitBeforeFirstSpawn());
        }
    }

    private void Update()
    {
        if (NumPrefabs >= maxSpawnedPrefabs || !WaitBeforeFirstSpawnCompleted)
        {
            return;
        }

        SpawnTimer += Time.deltaTime;

        if (SpawnTimer >= timeBetweenSpawns)
        {
            SpawnTimer = 0f;
            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        Vector2 spawnPositionGenerator()
        {
            float spawnXPosition = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
            float spawnYPosition = Random.Range(minSpawnPosition.y, maxSpawnPosition.y);
            return new Vector2(spawnXPosition, spawnYPosition);
        }

        Vector2 spawnPosition;

        if (prefabToSpawnBoxCollider.isTrigger)
        {
            spawnPosition = spawnPositionGenerator();
        }
        else if (!SpawnColliderHelper.TryGetSpawnPositionOutsideColliders(spawnPositionGenerator, prefabToSpawnColliderExtents,
            maxSpawnAttempts, out spawnPosition))
        {
            return;
        }

        GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
        spawnedPrefab.GetComponent<Spawnable>().SetSpawner(this);

        ApplySpawnedPrefabProperties(spawnedPrefab);

        NumPrefabs++;
    }

    private IEnumerator WaitBeforeFirstSpawn()
    {
        yield return beforeFirstSpawnWaitForSeconds;

        WaitBeforeFirstSpawnCompleted = true;

        NumPrefabs = 0;
        SpawnTimer = 0f;

        if (NumPrefabs < maxSpawnedPrefabs)
        {
            SpawnPrefab();
        }
    }

    protected virtual void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
    }

    public void SpawnedPrefabDestroyed()
    {
        NumPrefabs--;
    }

    public string GetSaveGuid() =>
        spawnerSaveManager != null ? spawnerSaveManager.GetSaveGuid() : string.Empty;

    public Vector2 GetMinSpawnPosition() => minSpawnPosition;

    public Vector2 GetMaxSpawnPosition() => maxSpawnPosition;
}
