using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerSaveManager : MonoBehaviour
{
    private PrefabSpawner[] prefabSpawners;

    private void Awake()
    {
        prefabSpawners = FindObjectsOfType<PrefabSpawner>();
    }

    public SpawnerSaveEntry[] GetSpawnerSaveEntries()
    {
        SpawnerSaveEntry[] spawnerSaveEntries =
            prefabSpawners.Select(GetSpawnerSaveEntryFromSpawner).ToArray();

        return spawnerSaveEntries;
    }

    public void SetSpawnerStates(SpawnerSaveEntry[] spawnerSaveEntries)
    {
        List<PrefabSpawner> spawners = FindObjectsOfType<PrefabSpawner>().ToList();

        foreach (var spawnerSaveEntry in spawnerSaveEntries)
        {
            PrefabSpawner spawner = spawners.Find(
                x => x.GetSpawnerId() == spawnerSaveEntry.SpawnerId);

            spawner.SetPropertiesFromSerializableState(spawnerSaveEntry.State);
        }
    }

    private SpawnerSaveEntry GetSpawnerSaveEntryFromSpawner(PrefabSpawner spawner)
    {
        var spawnerSaveEntry = new SpawnerSaveEntry
        {
            SpawnerId = spawner.GetSpawnerId(),
            State = spawner.GetSerializableState()
        };

        return spawnerSaveEntry;
    }

    [Serializable]
    public class SpawnerSaveEntry
    {
        public int SpawnerId;
        public PrefabSpawner.SerializableSpawnerState State;
    }
}
