using System;
using System.IO;
using System.Linq;
using UnityEngine;
using static Chimp;
using static EnemySaveManager;
using static Inventory;
using static ItemWorldSaveManager;
using static PlayerController;
using static SpawnerSaveManager;

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
            debugAddItemUISaveManager.GetSavableState()
        };

        var saveData = new SaveData
        {
            SavablePlayerState =
                (SavablePlayerState)playerController.GetSavableState(),
            SavableInventoryState =
                (SavableInventoryState)playerInventory.GetSavableState(),
            SavableChimpState = (SavableChimpState)chimp.GetSavableState(),
            SpawnerSaveManagerState =
                (SavableSpawnerSaveManagerState)spawnerSaveManager.GetSavableState(),
            ItemWorldSaveManagerState =
                (SavableItemWorldSaveManagerState)itemWorldSaveManager.GetSavableState(),
            EnemySaveManagerState =
                (SavableEnemySaveManagerState)enemySaveManager.GetSavableState(),
            SavableStates = savableStates
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

        playerController
            .SetPropertiesFromSavableState(saveData.SavablePlayerState);

        playerInventory
            .SetPropertiesFromSavableState(saveData.SavableInventoryState);

        chimp.SetPropertiesFromSavableState(saveData.SavableChimpState);

        spawnerSaveManager
            .SetPropertiesFromSavableState(saveData.SpawnerSaveManagerState);

        itemWorldSaveManager
            .SetPropertiesFromSavableState(saveData.ItemWorldSaveManagerState);

        enemySaveManager
            .SetPropertiesFromSavableState(saveData.EnemySaveManagerState);

        foreach (var savableState in saveData.SavableStates)
        {
            Type savableClassType = savableState.GetSavableClassType();

            ISavable[] foundSavableObjects =
                (ISavable[])FindObjectsOfType(savableClassType);

            ISavable savableObject = foundSavableObjects.FirstOrDefault(
                x => x.GetSaveGuid() == savableState.SaveGuid);

            savableObject.SetPropertiesFromSavableState(savableState);
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
        public SavablePlayerState SavablePlayerState;
        public SavableInventoryState SavableInventoryState;
        public SavableChimpState SavableChimpState;
        public SavableSpawnerSaveManagerState SpawnerSaveManagerState;
        public SavableItemWorldSaveManagerState ItemWorldSaveManagerState;
        public SavableEnemySaveManagerState EnemySaveManagerState;

        [SerializeReference]
        public SavableState[] SavableStates;
    }

    public static string GetSaveDataFilePath() =>
        Application.persistentDataPath + Path.DirectorySeparatorChar + saveDataFileName;
}
