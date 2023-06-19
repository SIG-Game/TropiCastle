using System.Linq;
using UnityEngine;
using static EnemyController;

public class EnemySaveManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;

    public SerializableEnemyState[] GetEnemyStates()
    {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();

        SerializableEnemyState[] enemyStates =
            enemyControllers.Select(x => x.GetSerializableState()).ToArray();

        return enemyStates;
    }

    public void CreateEnemiesFromStates(SerializableEnemyState[] enemyStates)
    {
        EnemySpawner[] enemySpawners = FindObjectsOfType<EnemySpawner>();

        foreach (SerializableEnemyState enemyState in enemyStates)
        {
            GameObject spawnedEnemy =
                Instantiate(enemyPrefab, enemyState.Position, Quaternion.identity);

            EnemyController spawnedEnemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            spawnedEnemyController.SetUpEnemy(playerTransform, playerInventory);

            if (enemyState.SpawnerId != -1)
            {
                EnemySpawner enemySpawner = enemySpawners.FirstOrDefault(
                    x => x.GetInstanceID() == enemyState.SpawnerId);

                if (enemySpawner != null)
                {
                    spawnedEnemy.GetComponent<Spawnable>().SetSpawner(enemySpawner);
                }
            }
        }
    }
}
