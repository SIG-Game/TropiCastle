using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PrefabSpawner;

public class SpawnerSaveManager : MonoBehaviour,
    ISaveManager<SavableSpawnerState>
{
    private PrefabSpawner[] prefabSpawners;

    private void Awake()
    {
        prefabSpawners = FindObjectsOfType<PrefabSpawner>();
    }

    public SavableSpawnerState[] GetStates()
    {
        SavableSpawnerState[] spawnerStates = prefabSpawners.Select(
            x => (SavableSpawnerState)x.GetSavableState()).ToArray();

        return spawnerStates;
    }

    public void CreateObjectsFromStates(SavableSpawnerState[] states)
    {
        List<PrefabSpawner> spawners = FindObjectsOfType<PrefabSpawner>().ToList();

        foreach (SavableSpawnerState state in states)
        {
            PrefabSpawner spawner = spawners.Find(
                x => x.GetSaveGuid() == state.SaveGuid);

            spawner.SetPropertiesFromSavableState(state);
        }
    }
}
