using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PrefabSpawner;

public class SpawnerSaveManager : MonoBehaviour,
    ISaveManager<SerializableSpawnerState>
{
    private PrefabSpawner[] prefabSpawners;

    private void Awake()
    {
        prefabSpawners = FindObjectsOfType<PrefabSpawner>();
    }

    public SerializableSpawnerState[] GetStates()
    {
        SerializableSpawnerState[] spawnerStates =
            prefabSpawners.Select(x => x.GetSerializableState()).ToArray();

        return spawnerStates;
    }

    public void CreateObjectsFromStates(SerializableSpawnerState[] states)
    {
        List<PrefabSpawner> spawners = FindObjectsOfType<PrefabSpawner>().ToList();

        foreach (SerializableSpawnerState state in states)
        {
            PrefabSpawner spawner = spawners.Find(
                x => x.GetSpawnerId() == state.SpawnerId);

            spawner.SetPropertiesFromSerializableState(state);
        }
    }
}
