using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private SavableItemWorldDependencySetter itemWorldDependencySetter;
    [SerializeField] private SavableEnemyDependencySetter enemyDependencySetter;

    private Dictionary<Type, ISavablePrefabInstanceDependencySetter> prefabTypeToDependencySetter;
    private string saveDataFilePath;

    private const string saveDataFileName = "save_data.json";

    private void Awake()
    {
        prefabTypeToDependencySetter =
            new Dictionary<Type, ISavablePrefabInstanceDependencySetter>
            {
                { typeof(ItemWorld), itemWorldDependencySetter },
                { typeof(EnemyController), enemyDependencySetter }
            };

        saveDataFilePath = GetSaveDataFilePath();
    }

    private void Start()
    {
        // This method call must be in the Start method so that it runs
        // after events in the Inventory class have been subscribed to
        LoadFromFile();
    }

    public void SaveToFile()
    {
        var savableStates = new SavableState[]
        {
            playerInventory.GetSavableState()
        };

        SaveManager[] saveManagers = FindObjectsOfType<SaveManager>();

        SavableState[] saveManagerStates = saveManagers.Select(
            x => x.GetSavableState()).ToArray();

        savableStates = savableStates.Concat(saveManagerStates).ToArray();

        ItemWorld[] itemWorlds = FindObjectsOfType<ItemWorld>();
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        SavablePrefabInstanceState[] itemWorldStates = itemWorlds.Select(
            x => x.GetSavablePrefabInstanceState()).ToArray();
        SavablePrefabInstanceState[] enemyStates = enemies.Select(
            x => x.GetSavablePrefabInstanceState()).ToArray();

        SavablePrefabInstanceState[] savablePrefabInstanceStates =
            itemWorldStates.Concat(enemyStates).ToArray();

        var saveData = new SaveData
        {
            SavableStates = savableStates,
            SavablePrefabInstanceStates = savablePrefabInstanceStates
        };

        WriteSerializableObjectAsJsonToFile(saveData, saveDataFilePath);
    }

    private void LoadFromFile()
    {
        if (!File.Exists(saveDataFilePath))
        {
            return;
        }

        var saveData =
            GetSerializableObjectFromJsonFile<SaveData>(saveDataFilePath);

        foreach (var savableState in saveData.SavableStates)
        {
            Type savableClassType = savableState.GetSavableClassType();

            ISavable[] foundSavableObjects =
                (ISavable[])FindObjectsOfType(savableClassType);

            ISavable savableObject = foundSavableObjects.FirstOrDefault(
                x => x.GetSaveGuid() == savableState.SaveGuid);

            savableObject.SetPropertiesFromSavableState(savableState);
        }

        var savablePrefabsLoadHandle = Addressables
            .LoadAssetsAsync<GameObject>("savable prefab", null);

        List<GameObject> savablePrefabs = new List<GameObject>(
            savablePrefabsLoadHandle.WaitForCompletion());

        foreach (var savablePrefabInstanceState in saveData.SavablePrefabInstanceStates)
        {
            string savablePrefabName = savablePrefabInstanceState.GetSavablePrefabName();

            GameObject savablePrefab = savablePrefabs.Find(x => x.name == savablePrefabName);

            GameObject spawnedGameObject = Instantiate(savablePrefab);

            ISavablePrefabInstance savablePrefabInstance =
                spawnedGameObject.GetComponent<ISavablePrefabInstance>();

            ISavablePrefabInstanceDependencySetter dependencySetter =
                prefabTypeToDependencySetter[savablePrefabInstance.GetType()];

            // Must run before SetPropertiesFromSavablePrefabInstanceState
            dependencySetter.SetPrefabInstanceDependencies(savablePrefabInstance);

            savablePrefabInstance
                .SetPropertiesFromSavablePrefabInstanceState(savablePrefabInstanceState);
        }

        if (savablePrefabsLoadHandle.IsValid())
        {
            Addressables.Release(savablePrefabsLoadHandle);
        }
    }

    private void WriteSerializableObjectAsJsonToFile(object serializableObject, string filePath)
    {
        using (var streamWriter = new StreamWriter(filePath))
        {
            string serializableObjectJson = JsonUtility.ToJson(serializableObject);

            streamWriter.Write(serializableObjectJson);
        }
    }

    private TSerializableObject GetSerializableObjectFromJsonFile<TSerializableObject>(
        string filePath)
    {
        TSerializableObject serializableObject;

        using (var streamReader = new StreamReader(filePath))
        {
            string serializableObjectJson = streamReader.ReadToEnd();

            serializableObject = JsonUtility
                .FromJson<TSerializableObject>(serializableObjectJson);
        }

        return serializableObject;
    }

    [Serializable]
    private class SaveData
    {
        [SerializeReference]
        public SavableState[] SavableStates;

        [SerializeReference]
        public SavablePrefabInstanceState[] SavablePrefabInstanceStates;
    }

    public static string GetSaveDataFilePath() =>
        Application.persistentDataPath + Path.DirectorySeparatorChar + saveDataFileName;
}
