using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PrefabSpawner;

public class SpawnerSaveManager : MonoBehaviour, ISavable
{
    private PrefabSpawner[] prefabSpawners;

    private void Awake()
    {
        prefabSpawners = FindObjectsOfType<PrefabSpawner>();
    }

    public SavableState GetSavableState()
    {
        SavableSpawnerState[] spawnerStates = prefabSpawners.Select(
            x => (SavableSpawnerState)x.GetSavableState()).ToArray();

        var savableState = new SavableSpawnerSaveManagerState
        {
            SpawnerStates = spawnerStates
        };

        return savableState;
    }

    public void SetPropertiesFromSavableState(SavableState savableState)
    {
        var spawnerSaveManagerState =
            (SavableSpawnerSaveManagerState)savableState;

        List<PrefabSpawner> spawners = FindObjectsOfType<PrefabSpawner>().ToList();

        foreach (var spawnerState in spawnerSaveManagerState.SpawnerStates)
        {
            PrefabSpawner spawner = spawners.Find(
                x => x.GetSaveGuid() == spawnerState.SaveGuid);

            spawner.SetPropertiesFromSavableState(spawnerState);
        }
    }

    [Serializable]
    public class SavableSpawnerSaveManagerState : SavableState
    {
        public SavableSpawnerState[] SpawnerStates;
    }
}
