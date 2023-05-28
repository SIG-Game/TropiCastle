using System;
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
        foreach (var spawnerSaveEntry in spawnerSaveEntries)
        {
            GameObject spawner = GameObject.Find(spawnerSaveEntry.SpawnerName);

            spawner.GetComponent<PrefabSpawner>()
                .SetStateFromSerializableState(spawnerSaveEntry.State);
        }
    }

    private SpawnerSaveEntry GetSpawnerSaveEntryFromSpawner(PrefabSpawner spawner)
    {
        var spawnerSaveEntry = new SpawnerSaveEntry
        {
            SpawnerName = spawner.gameObject.name,
            State = spawner.GetSerializableSpawnerState()
        };

        // Because GameObjects with a Spawnable component are not saved, NumberOfSpawnedPrefabs
        // needs to be set to 0 so that counts of spawned GameObjects for spawners are accurate
        // after loading a save.
        // TODO: Remove when GameObjects with Spawnable components are saved
        spawnerSaveEntry.State.NumberOfSpawnedPrefabs = 0;

        return spawnerSaveEntry;
    }

    [Serializable]
    public class SpawnerSaveEntry
    {
        public string SpawnerName;
        public PrefabSpawner.SerializableSpawnerState State;
    }
}
