using System.Linq;
using UnityEngine;
using static EnemyController;

public class EnemySaveManager : MonoBehaviour,
    ISaveManager<SerializableEnemyState>
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;

    public SerializableEnemyState[] GetStates()
    {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();

        SerializableEnemyState[] states =
            enemyControllers.Select(x => x.GetSerializableState()).ToArray();

        return states;
    }

    public void CreateObjectsFromStates(SerializableEnemyState[] states)
    {
        foreach (SerializableEnemyState state in states)
        {
            GameObject spawnedEnemy = Instantiate(enemyPrefab);

            EnemyController spawnedEnemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            spawnedEnemyController.SetUpEnemy(playerTransform, playerInventory);

            spawnedEnemyController.SetPropertiesFromSerializableState(state);
        }
    }
}
