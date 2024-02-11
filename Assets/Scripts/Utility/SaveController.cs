using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SaveController : MonoBehaviour
{
    private JsonSerializerSettings serializerSettings;
    private string saveDataFilePath;

    private const string saveDataFileName = "save_data.json";

    private void Awake()
    {
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

        using (var streamWriter = new StreamWriter(saveDataFilePath))
        {
#if UNITY_EDITOR
            string saveDataJson = JsonConvert.SerializeObject(
                saveData, Formatting.Indented, serializerSettings);
#else
            string saveDataJson = JsonConvert.SerializeObject(
                saveData, serializerSettings);
#endif

            streamWriter.Write(saveDataJson);
        }
    }

    private void LoadFromFile()
    {
        if (!File.Exists(saveDataFilePath))
        {
            return;
        }

        SaveData saveData;

        using (var streamReader = new StreamReader(saveDataFilePath))
        {
            string serializableObjectJson = streamReader.ReadToEnd();

            saveData = JsonConvert.DeserializeObject<SaveData>(
                serializableObjectJson, serializerSettings);
        }

        SaveManager[] saveManagers = FindObjectsOfType<SaveManager>();

        foreach (var saveManagerState in saveData.SaveManagerStates)
        {
            SaveManager saveManager = saveManagers.FirstOrDefault(
                x => x.GetSaveGuid() == saveManagerState.SaveGuid);

            saveManager.UpdateFromProperties(saveManagerState.Properties);
        }

        var savablePrefabGameObjectsLoadHandle = Addressables
            .LoadAssetsAsync<GameObject>("savable prefab", null);

        Dictionary<string, GameObject> savablePrefabGameObjects =
            savablePrefabGameObjectsLoadHandle
                .WaitForCompletion().ToDictionary(x => x.name);

        foreach (var savablePrefabState in saveData.SavablePrefabStates)
        {
            GameObject savablePrefabGameObject = savablePrefabGameObjects[
                savablePrefabState.PrefabGameObjectName];

            GameObject spawnedGameObject = Instantiate(savablePrefabGameObject);

            SavablePrefab savablePrefab =
                spawnedGameObject.GetComponent<SavablePrefab>();

            savablePrefab.SetUpFromProperties(savablePrefabState.Properties);
        }

        if (savablePrefabGameObjectsLoadHandle.IsValid())
        {
            Addressables.Release(savablePrefabGameObjectsLoadHandle);
        }
    }

    [Serializable]
    private class SaveData
    {
        public SaveManagerState[] SaveManagerStates;
        public SavablePrefabState[] SavablePrefabStates;
    }

    public static string GetSaveDataFilePath() =>
        Application.persistentDataPath + Path.DirectorySeparatorChar + saveDataFileName;
}
