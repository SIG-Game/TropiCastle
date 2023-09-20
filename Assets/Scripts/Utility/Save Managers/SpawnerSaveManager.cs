using System;
using UnityEngine;

public class SpawnerSaveManager : SaveManager
{
    [SerializeField] private PrefabSpawner spawner;

    public override SaveManagerState GetState()
    {
        var saveManagerState = new SpawnerSaveManagerState
        {
            SaveGuid = saveGuid,
            NumberOfSpawnedPrefabs = spawner.NumPrefabs,
            SpawnTimer = spawner.SpawnTimer,
            WaitBeforeFirstSpawnCompleted = spawner.WaitBeforeFirstSpawnCompleted
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        SpawnerSaveManagerState spawnerState = (SpawnerSaveManagerState)saveManagerState;

        spawner.NumPrefabs = spawnerState.NumberOfSpawnedPrefabs;
        spawner.SpawnTimer = spawnerState.SpawnTimer;
        spawner.WaitBeforeFirstSpawnCompleted = spawnerState.WaitBeforeFirstSpawnCompleted;
    }

    [Serializable]
    public class SpawnerSaveManagerState : SaveManagerState
    {
        public int NumberOfSpawnedPrefabs;
        public float SpawnTimer;
        public bool WaitBeforeFirstSpawnCompleted;
    }
}
