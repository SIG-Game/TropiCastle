using System;
using System.Collections.Generic;
using System.Linq;
using static PrefabSpawner;

public class SpawnerSaveManager : SaveManager
{
    private PrefabSpawner[] prefabSpawners;

    private void Awake()
    {
        prefabSpawners = FindObjectsOfType<PrefabSpawner>();
    }

    public override SavableState GetSavableState()
    {
        SavableSpawnerState[] spawnerStates = prefabSpawners.Select(
            x => (SavableSpawnerState)x.GetSavableState()).ToArray();

        var savableState = new SavableSpawnerSaveManagerState
        {
            SaveGuid = saveGuid,
            SpawnerStates = spawnerStates
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
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

        public override Type GetSavableClassType() =>
            typeof(SpawnerSaveManager);
    }
}
