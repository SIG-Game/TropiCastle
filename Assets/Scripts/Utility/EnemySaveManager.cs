using System;
using System.Linq;
using UnityEngine;
using static EnemyController;

public class EnemySaveManager : SaveManager
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;

    public override SavableState GetSavableState()
    {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();

        SavableEnemyState[] enemyStates = enemyControllers.Select(
            x => (SavableEnemyState)x.GetSavableState()).ToArray();

        var savableState = new SavableEnemySaveManagerState
        {
            SaveGuid = saveGuid,
            EnemyStates = enemyStates
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
    {
        var enemySaveManagerState =
            (SavableEnemySaveManagerState)savableState;

        foreach (var enemyState in enemySaveManagerState.EnemyStates)
        {
            GameObject spawnedEnemy = Instantiate(enemyPrefab);

            EnemyController spawnedEnemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            spawnedEnemyController.SetUpEnemy(playerTransform, playerInventory);

            spawnedEnemyController.SetPropertiesFromSavableState(enemyState);
        }
    }

    [Serializable]
    public class SavableEnemySaveManagerState : SavableState
    {
        public SavableEnemyState[] EnemyStates;

        public override Type GetSavableClassType() =>
            typeof(EnemySaveManager);
    }
}
