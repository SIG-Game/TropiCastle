using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Inventory;
using static PlayerController;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private PlayerController playerController;

    private string inventoryFilePath;
    private string playerFilePath;

    private const string inventoryFileName = "inventory.json";
    private const string playerFileName = "player.json";

    private void Awake()
    {
        inventoryFilePath = Application.persistentDataPath +
            Path.DirectorySeparatorChar + inventoryFileName;

        playerFilePath = Application.persistentDataPath +
            Path.DirectorySeparatorChar + playerFileName;
    }

    private void Start()
    {
        // This method call must be in the Start method so that it runs
        // after events in the Inventory class have been subscribed to
        LoadInventoryFromFile();

        LoadPlayerPropertiesFromFile();
    }

    public void SaveInventoryToFile()
    {
        var serializableInventory = playerInventory.GetSerializableInventory();

        WriteSerializableObjectAsJsonToFile(serializableInventory, inventoryFilePath);
    }

    public void SavePlayerPropertiesToFile()
    {
        Vector2 playerPosition = playerController.transform.position;
        int playerDirection = (int)playerController.Direction;

        var serializablePlayerProperties = new SerializablePlayerProperties
        {
            PlayerPosition = playerPosition,
            PlayerDirection = playerDirection
        };

        WriteSerializableObjectAsJsonToFile(serializablePlayerProperties, playerFilePath);
    }

    private void LoadInventoryFromFile()
    {
        if (!File.Exists(inventoryFilePath))
        {
            return;
        }

        var serializableInventory =
            GetSerializableObjectFromJsonFile<SerializableInventory>(inventoryFilePath);

        playerInventory.SetInventoryFromSerializableInventory(serializableInventory);
    }

    private void LoadPlayerPropertiesFromFile()
    {
        if (!File.Exists(playerFilePath))
        {
            return;
        }

        var serializablePlayerProperties =
            GetSerializableObjectFromJsonFile<SerializablePlayerProperties>(playerFilePath);

        playerController
            .SetPropertiesFromSerializablePlayerProperties(serializablePlayerProperties);
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
}
