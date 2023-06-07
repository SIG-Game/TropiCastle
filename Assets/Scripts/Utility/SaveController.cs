using System;
using System.IO;
using UnityEngine;
using static Chimp;
using static EnemyController;
using static Inventory;
using static ItemWorld;
using static PlayerController;
using static SpawnerSaveManager;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnerSaveManager spawnerSaveManager;
    [SerializeField] private ItemWorldSaveManager itemWorldSaveManager;
    [SerializeField] private EnemySaveManager enemySaveManager;
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
            SerializableInventory = playerInventory.GetSerializableInventory(),
            SerializablePlayerProperties = playerController.GetSerializablePlayerProperties(),
            SpawnerSaveEntries = spawnerSaveManager.GetSpawnerSaveEntries(),
            itemWorldStates = itemWorldSaveManager.GetItemWorldStates(),
            enemyStates = enemySaveManager.GetEnemyStates(),
            chimpState = chimp.GetSerializableChimpState()
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
            .SetInventoryFromSerializableInventory(saveData.SerializableInventory);

        playerController
            .SetPropertiesFromSerializablePlayerProperties(saveData.SerializablePlayerProperties);

        spawnerSaveManager.SetSpawnerStates(saveData.SpawnerSaveEntries);

        itemWorldSaveManager.CreateItemWorldsFromStates(saveData.itemWorldStates);

        enemySaveManager.CreateEnemiesFromStates(saveData.enemyStates);

        chimp.SetPropertiesFromSerializableChimpState(saveData.chimpState);
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
        public SpawnerSaveEntry[] SpawnerSaveEntries;
        public SerializableItemWorldState[] itemWorldStates;
        public SerializableEnemyState[] enemyStates;
        public SerializableChimpState chimpState;
    }
}
