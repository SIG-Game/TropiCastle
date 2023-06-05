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

        return spawnerSaveEntry;
    }

    [Serializable]
    public class SpawnerSaveEntry
    {
        public string SpawnerName;
        public PrefabSpawner.SerializableSpawnerState State;
    }
}
