using System;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateEnum;

public class SavablePrefabEnemy : SavablePrefab
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private HealthController healthController;
    [SerializeField] private Spawnable spawnable;

    public override string PrefabGameObjectName => "Crab";

    public override Dictionary<string, object> GetProperties()
    {
        EnemyStateEnum enemyState = enemyController.CurrentState switch
        {
            InitialEnemyState => Initial,
            IdleEnemyState => Idle,
            ChasingEnemyState => Chasing,
            KnockedBackEnemyState => KnockedBack,
            FadingOutEnemyState => FadingOut,
            _ => Idle
        };

        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToArray() },
            { "Health", healthController.Health },
            { "SpawnerGuid", spawnable.GetSpawnerGuid() },
            { "EnemyState", enemyState }
        };

        return properties;
    }

    public override void SetUpFromProperties(Dictionary<string, object> properties)
    {
        transform.position = Vector3Helper.FromArray((float[])properties["Position"]);

        healthController.SetInitialHealth(Convert.ToInt32(properties["Health"]));

        spawnable.SetSpawnerUsingGuid<PrefabSpawner>((string)properties["SpawnerGuid"]);

        enemyController.SetInitialStateFromEnum(
            (EnemyStateEnum)Convert.ToInt32(properties["EnemyState"]));
    }
}
