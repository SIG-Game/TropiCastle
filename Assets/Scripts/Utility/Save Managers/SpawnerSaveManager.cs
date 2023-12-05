using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSaveManager : SaveManager
{
    [SerializeField] private PrefabSpawner spawner;

    public override Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>
        {
            { "NumberOfSpawnedPrefabs", spawner.NumPrefabs },
            { "SpawnTimer", spawner.SpawnTimer },
            { "WaitBeforeFirstSpawnCompleted",
                spawner.WaitBeforeFirstSpawnCompleted }
        };

        return properties;
    }

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        spawner.NumPrefabs = Convert.ToInt32(properties["NumberOfSpawnedPrefabs"]);
        spawner.SpawnTimer = Convert.ToSingle(properties["SpawnTimer"]);
        spawner.WaitBeforeFirstSpawnCompleted =
            (bool)properties["WaitBeforeFirstSpawnCompleted"];
    }
}
