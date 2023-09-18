using System;
using UnityEngine;

public class SpawnerSaveManager : SaveManager
{
    [SerializeField] private PrefabSpawner spawner;

    public override SavableState GetSavableState()
    {
        var savableState = new SavableSpawnerState
        {
            SaveGuid = saveGuid,
            NumberOfSpawnedPrefabs = spawner.NumPrefabs,
            SpawnTimer = spawner.SpawnTimer,
            WaitBeforeFirstSpawnCompleted = spawner.WaitBeforeFirstSpawnCompleted
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
    {
        SavableSpawnerState spawnerState = (SavableSpawnerState)savableState;

        spawner.NumPrefabs = spawnerState.NumberOfSpawnedPrefabs;
        spawner.SpawnTimer = spawnerState.SpawnTimer;
        spawner.WaitBeforeFirstSpawnCompleted = spawnerState.WaitBeforeFirstSpawnCompleted;
    }

    [Serializable]
    public class SavableSpawnerState : SavableState
    {
        public int NumberOfSpawnedPrefabs;
        public float SpawnTimer;
        public bool WaitBeforeFirstSpawnCompleted;

        public override Type GetSavableClassType() =>
            typeof(SpawnerSaveManager);
    }
}
