using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private SavableItemWorldDependencySetter itemWorldDependencySetter;
    [SerializeField] private SavableEnemyDependencySetter enemyDependencySetter;

    private Dictionary<Type, ISavablePrefabDependencySetter> typeToDependencySetter;
    private JsonSerializerSettings serializerSettings;
    private string saveDataFilePath;

    private const string saveDataFileName = "save_data.json";

    private void Awake()
    {
        typeToDependencySetter =
            new Dictionary<Type, ISavablePrefabDependencySetter>
            {
                { typeof(SavableItemWorldDependencySetter), itemWorldDependencySetter },
                { typeof(SavableEnemyDependencySetter), enemyDependencySetter }
            };

        serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
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
        SaveManager[] saveManagers = FindObjectsOfType<SaveManager>();

        SaveManagerState[] saveManagerStates =
            saveManagers.Select(x => x.GetState()).ToArray();

        SavablePrefab[] savablePrefabs = FindObjectsOfType<SavablePrefab>();

        SavablePrefabState[] savablePrefabStates = savablePrefabs.Select(
            x => x.GetSavablePrefabState()).ToArray();

        var saveData = new SaveData
        {
            SaveManagerStates = saveManagerStates,
            SavablePrefabStates = savablePrefabStates
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

        SaveManager[] saveManagers = FindObjectsOfType<SaveManager>();

        foreach (var saveManagerState in saveData.SaveManagerStates)
        {
            SaveManager saveManager = saveManagers.FirstOrDefault(
                x => x.GetSaveGuid() == saveManagerState.SaveGuid);

            saveManager.UpdateFromState(saveManagerState);
        }

        var savablePrefabGameObjectsLoadHandle = Addressables
            .LoadAssetsAsync<GameObject>("savable prefab", null);

        List<GameObject> savablePrefabGameObjects = new List<GameObject>(
            savablePrefabGameObjectsLoadHandle.WaitForCompletion());

        foreach (var savablePrefabState in saveData.SavablePrefabStates)
        {
            GameObject savablePrefabGameObject =
                savablePrefabGameObjects.Find(
                    x => x.name == savablePrefabState.PrefabGameObjectName);

            GameObject spawnedGameObject = Instantiate(savablePrefabGameObject);

            SavablePrefab savablePrefab =
                spawnedGameObject.GetComponent<SavablePrefab>();

            ISavablePrefabDependencySetter dependencySetter =
                typeToDependencySetter[savablePrefab.GetDependencySetterType()];

            // Must run before SetUpFromSavablePrefabState
            dependencySetter.SetPrefabDependencies(savablePrefab);

            savablePrefab.SetUpFromSavablePrefabState(savablePrefabState);
        }

        if (savablePrefabGameObjectsLoadHandle.IsValid())
        {
            Addressables.Release(savablePrefabGameObjectsLoadHandle);
        }
    }

    private void WriteSerializableObjectAsJsonToFile(object serializableObject, string filePath)
    {
        using (var streamWriter = new StreamWriter(filePath))
        {
#if UNITY_EDITOR
            string serializableObjectJson = JsonConvert.SerializeObject(
                serializableObject, Formatting.Indented, serializerSettings);
#else
            string serializableObjectJson =
                JsonConvert.SerializeObject(serializableObject, serializerSettings);
#endif

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

            serializableObject = JsonConvert.DeserializeObject<TSerializableObject>(
                serializableObjectJson, serializerSettings);
        }

        return serializableObject;
    }

    [Serializable]
    private class SaveData
    {
        [SerializeReference]
        public SaveManagerState[] SaveManagerStates;

        [SerializeReference]
        public SavablePrefabState[] SavablePrefabStates;
    }

    public static string GetSaveDataFilePath() =>
        Application.persistentDataPath + Path.DirectorySeparatorChar + saveDataFileName;
}
