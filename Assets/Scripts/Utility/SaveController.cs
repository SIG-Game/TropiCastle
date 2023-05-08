using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Inventory;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;

    private List<ItemWithAmount> playerInventoryItemList;
    private string inventoryFilePath;

    private const string inventoryFileName = "inventory.json";

    private void Awake()
    {
        inventoryFilePath = Application.persistentDataPath +
            Path.DirectorySeparatorChar + inventoryFileName;
    }

    private void Start()
    {
        playerInventoryItemList = playerInventory.GetItemList();

        // This method call must be in the Start method so that it runs
        // after events in the Inventory class have been subscribed to
        LoadInventoryFromFile();
    }

    public void SaveInventoryToFile()
    {
        IEnumerable<SerializableInventoryItem> serializableInventoryItems =
            playerInventoryItemList.Select(x => new SerializableInventoryItem
            {
                // Use ScriptableObject name and not item display name
                ItemName = ((ScriptableObject)x.itemData).name,
                Amount = x.amount
            });

        var serializableInventory = new SerializableInventory
        {
            SerializableItemList = serializableInventoryItems.ToList()
        };

        using (var streamWriter = new StreamWriter(inventoryFilePath))
        {
            string serializableInventoryJson =
                JsonUtility.ToJson(serializableInventory);

            streamWriter.Write(serializableInventoryJson);
        }
    }

    private void LoadInventoryFromFile()
    {
        if (!File.Exists(inventoryFilePath))
        {
            return;
        }

        using (var streamReader = new StreamReader(inventoryFilePath))
        {
            string inventoryJson = streamReader.ReadToEnd();

            var serializableInventory = JsonUtility
                .FromJson<SerializableInventory>(inventoryJson);

            playerInventory
                .SetInventoryFromSerializableInventory(serializableInventory);
        }
    }
}
