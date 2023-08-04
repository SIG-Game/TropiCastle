using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabSpawner : MonoBehaviour,
    ISavable<PrefabSpawner.SerializableSpawnerState>
{
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] private Vector2 timeBeforeFirstSpawnRange;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private int maxSpawnedPrefabs;
    [SerializeField] private Vector2 minSpawnPosition;
    [SerializeField] private Vector2 maxSpawnPosition;
    [SerializeField] private int spawnerId;
    [SerializeField] private bool logOnSpawn;
    [SerializeField] private bool drawSpawnArea;
    [SerializeField] private Color drawnSpawnAreaColor = Color.black;

    private int numPrefabs;
    private float spawnTimer;
    private bool waitBeforeFirstSpawnCompleted;
    private Vector3 topLeftCornerSpawnArea;
    private Vector3 topRightCornerSpawnArea;
    private Vector3 bottomLeftCornerSpawnArea;
    private Vector3 bottomRightCornerSpawnArea;
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

        topLeftCornerSpawnArea = new Vector3(minSpawnPosition.x, maxSpawnPosition.y);
        topRightCornerSpawnArea = new Vector3(maxSpawnPosition.x, maxSpawnPosition.y);
        bottomLeftCornerSpawnArea = new Vector3(minSpawnPosition.x, minSpawnPosition.y);
        bottomRightCornerSpawnArea = new Vector3(maxSpawnPosition.x, minSpawnPosition.y);

        prefabToSpawnBoxCollider = prefabToSpawn.GetComponent<BoxCollider2D>();
        prefabToSpawnColliderExtents = prefabToSpawnBoxCollider.size / 2f;
    }

    private void Start()
    {
        if (!waitBeforeFirstSpawnCompleted)
        {
            float beforeFirstSpawnWait = Random.Range(timeBeforeFirstSpawnRange.x,
            timeBeforeFirstSpawnRange.y);

            beforeFirstSpawnWaitForSeconds = new WaitForSeconds(beforeFirstSpawnWait);

            StartCoroutine(WaitBeforeFirstSpawn());
        }
    }

    private void Update()
    {
        if (drawSpawnArea)
        {
            Debug.DrawLine(topLeftCornerSpawnArea, topRightCornerSpawnArea, drawnSpawnAreaColor);
            Debug.DrawLine(topRightCornerSpawnArea, bottomRightCornerSpawnArea, drawnSpawnAreaColor);
            Debug.DrawLine(bottomRightCornerSpawnArea, bottomLeftCornerSpawnArea, drawnSpawnAreaColor);
            Debug.DrawLine(bottomLeftCornerSpawnArea, topLeftCornerSpawnArea, drawnSpawnAreaColor);
        }

        if (numPrefabs >= maxSpawnedPrefabs || !waitBeforeFirstSpawnCompleted)
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

        if (logOnSpawn)
        {
            Debug.Log($"Spawned {spawnedPrefab.name} at {spawnedPrefab.transform.position}.");
        }

        numPrefabs++;
    }

    private IEnumerator WaitBeforeFirstSpawn()
    {
        yield return beforeFirstSpawnWaitForSeconds;

        waitBeforeFirstSpawnCompleted = true;

        numPrefabs = 0;
        spawnTimer = 0f;

        if (numPrefabs < maxSpawnedPrefabs)
        {
            SpawnPrefab();
        }
    }

    protected virtual void ApplySpawnedPrefabProperties(GameObject spawnedPrefab)
    {
    }

    public void SpawnedPrefabDestroyed()
    {
        numPrefabs--;
    }

    public int GetSpawnerId() => spawnerId;

    public SerializableSpawnerState GetSerializableState()
    {
        var serializableState = new SerializableSpawnerState
        {
            SpawnerId = spawnerId,
            NumberOfSpawnedPrefabs = numPrefabs,
            SpawnTimer = spawnTimer,
            WaitBeforeFirstSpawnCompleted = waitBeforeFirstSpawnCompleted
        };

        return serializableState;
    }

    public void SetPropertiesFromSerializableState(
        SerializableSpawnerState serializableState)
    {
        spawnerId = serializableState.SpawnerId;
        numPrefabs = serializableState.NumberOfSpawnedPrefabs;
        spawnTimer = serializableState.SpawnTimer;
        waitBeforeFirstSpawnCompleted = serializableState.WaitBeforeFirstSpawnCompleted;
    }

    [Serializable]
    public class SerializableSpawnerState
    {
        public int SpawnerId;
        public int NumberOfSpawnedPrefabs;
        public float SpawnTimer;
        public bool WaitBeforeFirstSpawnCompleted;
    }
}
