using System;
using System.IO;
using UnityEngine;
using static Chimp;
using static DebugAddItemUISaveManager;
using static EnemyController;
using static Inventory;
using static ItemWorld;
using static PlayerController;
using static PrefabSpawner;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnerSaveManager spawnerSaveManager;
    [SerializeField] private ItemWorldSaveManager itemWorldSaveManager;
    [SerializeField] private EnemySaveManager enemySaveManager;
    [SerializeField] private DebugAddItemUISaveManager debugAddItemUISaveManager;
    [SerializeField] private Chimp chimp;

    private string saveDataFilePath;

    private const string saveDataFileName = "save_data.json";

    private void Awake()
    {
        saveDataFilePath = Application.persistentDataPath +
            Path.DirectorySeparatorChar + saveDataFileName;
    }

    private void Start()
    {
        // This method call must be in the Start method so that it runs
        // after events in the Inventory class have been subscribed to
        LoadFromFile();
    }

    public void SaveToFile()
    {
        var saveData = new SaveData
        {
            SerializableInventory = playerInventory.GetSerializableState(),
            SerializablePlayerProperties = playerController.GetSerializableState(),
            SpawnerStates = spawnerSaveManager.GetStates(),
            ItemWorldStates = itemWorldSaveManager.GetStates(),
            EnemyStates = enemySaveManager.GetStates(),
            DebugAddItemUIState = debugAddItemUISaveManager.GetSerializableState(),
            ChimpState = chimp.GetSerializableState()
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

        playerInventory
            .SetPropertiesFromSerializableState(saveData.SerializableInventory);

        playerController
            .SetPropertiesFromSerializableState(saveData.SerializablePlayerProperties);

        spawnerSaveManager.CreateObjectsFromStates(saveData.SpawnerStates);

        itemWorldSaveManager.CreateObjectsFromStates(saveData.ItemWorldStates);

        enemySaveManager.CreateObjectsFromStates(saveData.EnemyStates);

        debugAddItemUISaveManager
            .SetPropertiesFromSerializableState(saveData.DebugAddItemUIState);

        chimp.SetPropertiesFromSerializableState(saveData.ChimpState);
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
        public SerializableInventory SerializableInventory;
        public SerializablePlayerProperties SerializablePlayerProperties;
        public SerializableSpawnerState[] SpawnerStates;
        public SerializableItemWorldState[] ItemWorldStates;
        public SerializableEnemyState[] EnemyStates;
        public SerializableDebugAddItemUIState DebugAddItemUIState;
        public SerializableChimpState ChimpState;
    }
}
