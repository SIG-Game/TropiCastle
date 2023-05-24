using System;
using System.IO;
using UnityEngine;
using static Inventory;
using static PlayerController;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private PlayerController playerController;

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
            SerializableInventory = GetSerializableInventory(),
            SerializablePlayerProperties = GetSerializablePlayerProperties()
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
    }

    public SerializableInventory GetSerializableInventory()
    {
        var serializableInventory = playerInventory.GetSerializableInventory();

        return serializableInventory;
    }

    public SerializablePlayerProperties GetSerializablePlayerProperties()
    {
        Vector2 playerPosition = playerController.transform.position;
        int playerDirection = (int)playerController.Direction;
        int selectedItemIndex = playerController.GetSelectedItemIndex();

        var serializablePlayerProperties = new SerializablePlayerProperties
        {
            PlayerPosition = playerPosition,
            PlayerDirection = playerDirection,
            SelectedItemIndex = selectedItemIndex
        };

        return serializablePlayerProperties;
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
    }
}
