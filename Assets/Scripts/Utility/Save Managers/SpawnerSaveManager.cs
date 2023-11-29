using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSaveManager : SaveManager
{
    [SerializeField] private PrefabSpawner spawner;

    public override SaveManagerState GetState()
    {
        var properties = new Dictionary<string, object>
        {
            { "NumberOfSpawnedPrefabs", spawner.NumPrefabs },
            { "SpawnTimer", spawner.SpawnTimer },
            { "WaitBeforeFirstSpawnCompleted",
                spawner.WaitBeforeFirstSpawnCompleted }
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = properties
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        spawner.NumPrefabs =
            Convert.ToInt32(saveManagerState.Properties["NumberOfSpawnedPrefabs"]);
        spawner.SpawnTimer =
            Convert.ToSingle(saveManagerState.Properties["SpawnTimer"]);
        spawner.WaitBeforeFirstSpawnCompleted =
            (bool)saveManagerState.Properties["WaitBeforeFirstSpawnCompleted"];
    }
}
