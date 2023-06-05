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
        foreach (SerializableEnemyState enemyState in enemyStates)
        {
            GameObject spawnedEnemy =
                Instantiate(enemyPrefab, enemyState.Position, Quaternion.identity);

            EnemyController spawnedEnemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            spawnedEnemyController.SetPlayerTransform(playerTransform);
            spawnedEnemyController.SetPlayerInventory(playerInventory);

            if (!string.IsNullOrEmpty(enemyState.SpawnerGameObjectName))
            {
                GameObject enemySpawnerGameObject =
                    GameObject.Find(enemyState.SpawnerGameObjectName);

                spawnedEnemy.GetComponent<Spawnable>()
                    .SetSpawner(enemySpawnerGameObject.GetComponent<PrefabSpawner>());
            }
        }
    }
}
