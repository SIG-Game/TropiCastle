using System;
using System.Collections.Generic;
using UnityEngine;

public class SavablePrefabEnemy : SavablePrefab
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private HealthController healthController;
    [SerializeField] private Spawnable spawnable;

    public override SavablePrefabState GetSavablePrefabState()
    {
        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToArray() },
            { "Health", healthController.Health },
            { "SpawnerGuid", spawnable.GetSpawnerGuid() }
        };

        var savableState = new SavablePrefabState
        {
            PrefabGameObjectName = "Crab",
            Properties = properties
        };

        return savableState;
    }

    public override void SetUpFromSavablePrefabState(SavablePrefabState savableState)
    {
        transform.position =
            Vector3Helper.FromArray((float[])savableState.Properties["Position"]);

        healthController.SetInitialHealth(
            Convert.ToInt32(savableState.Properties["Health"]));

        spawnable.SetSpawnerUsingGuid<EnemySpawner>(
            (string)savableState.Properties["SpawnerGuid"]);
    }

    public override Type GetDependencySetterType() =>
        typeof(SavableEnemyDependencySetter);

    public EnemyController GetEnemyController() => enemyController;
}
