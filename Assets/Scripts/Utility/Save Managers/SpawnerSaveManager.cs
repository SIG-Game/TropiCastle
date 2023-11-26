using System.Collections.Generic;
using UnityEngine;

public class SpawnerSaveManager : SaveManager
{
    [SerializeField] private PrefabSpawner spawner;

    public override SaveManagerState GetState()
    {
        var propertyList = new List<Property>()
        {
            new Property("NumberOfSpawnedPrefabs", spawner.NumPrefabs.ToString()),
            new Property("SpawnTimer", spawner.SpawnTimer.ToString()),
            new Property("WaitBeforeFirstSpawnCompleted",
                spawner.WaitBeforeFirstSpawnCompleted.ToString())
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = new PropertyCollection(propertyList)
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        spawner.NumPrefabs = saveManagerState.Properties.GetIntProperty("NumberOfSpawnedPrefabs");
        spawner.SpawnTimer = saveManagerState.Properties.GetFloatProperty("SpawnTimer");
        spawner.WaitBeforeFirstSpawnCompleted =
            saveManagerState.Properties.GetBoolProperty("WaitBeforeFirstSpawnCompleted");
    }
}
