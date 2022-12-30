using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform player;
    [SerializeField] private float timeBetweenSpawns = 2f;
    [SerializeField] private int maxEnemies;

    private int numEnemies;
    private float spawnTimer;

    private void Awake()
    {
        numEnemies = 0;
        spawnTimer = 0f;

        if (numEnemies < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        if (numEnemies >= maxEnemies)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= timeBetweenSpawns)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        float randX = Random.Range(-8f, 8f);
        float randY = Random.Range(-3f, 4f);
        Vector2 spawnPosition = new Vector2(randX, randY);

        Debug.Log($"Spawned enemy at ({randX}, {randY}).");

        GameObject spawnedEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
        spawnedEnemy.GetComponent<enemyScript>().player = player;
        spawnedEnemy.GetComponent<enemyScript>().spawner = this;
        numEnemies++;
    }

    public void SpawnedEnemyDied()
    {
        numEnemies--;
    }
}
