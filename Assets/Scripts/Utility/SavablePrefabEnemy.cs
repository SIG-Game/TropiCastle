using System;
using System.Collections.Generic;
using UnityEngine;

public class SavablePrefabEnemy : SavablePrefab
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private HealthController healthController;
    [SerializeField] private Spawnable spawnable;

    public override string PrefabGameObjectName => "Crab";

    public override Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToArray() },
            { "Health", healthController.Health },
            { "SpawnerGuid", spawnable.GetSpawnerGuid() }
        };

        return properties;
    }

    public override void SetUpFromProperties(Dictionary<string, object> properties)
    {
        transform.position = Vector3Helper.FromArray((float[])properties["Position"]);

        healthController.SetInitialHealth(Convert.ToInt32(properties["Health"]));

        spawnable.SetSpawnerUsingGuid<PrefabSpawner>((string)properties["SpawnerGuid"]);
    }
}
