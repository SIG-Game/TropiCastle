using System.Linq;
using UnityEngine;
using static EnemyController;

public class EnemySaveManager : MonoBehaviour,
    ISaveManager<SavableEnemyState>
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;

    public SavableEnemyState[] GetStates()
    {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();

        SavableEnemyState[] states = enemyControllers.Select(
            x => (SavableEnemyState)x.GetSavableState()).ToArray();

        return states;
    }

    public void CreateObjectsFromStates(SavableEnemyState[] states)
    {
        foreach (SavableEnemyState state in states)
        {
            GameObject spawnedEnemy = Instantiate(enemyPrefab);

            EnemyController spawnedEnemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            spawnedEnemyController.SetUpEnemy(playerTransform, playerInventory);

            spawnedEnemyController.SetPropertiesFromSavableState(state);
        }
    }
}
