using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class InventorySaveManager : SaveManager
{
    [SerializeField] private Inventory inventory;

    public override SaveManagerState GetState()
    {
        SerializableInventory serializableInventory =
            inventory.GetAsSerializableInventory();

        var propertyList = new List<Property>
        {
            new Property("SerializedInventory",
                JsonUtility.ToJson(serializableInventory))
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = new PropertyCollection(propertyList)
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        string serializedInventory = saveManagerState.Properties
            .GetStringProperty("SerializedInventory");

        SerializableInventory serializableInventory = JsonUtility
            .FromJson<SerializableInventory>(serializedInventory);

        inventory.SetUpFromSerializableInventory(serializableInventory);
    }
}
