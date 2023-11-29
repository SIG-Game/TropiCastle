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

        var properties = new Dictionary<string, object>
        {
            { "SerializedInventory", serializableInventory }
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = properties
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        inventory.SetUpFromSerializableInventory((SerializableInventory)
            saveManagerState.Properties["SerializedInventory"]);
    }
}
