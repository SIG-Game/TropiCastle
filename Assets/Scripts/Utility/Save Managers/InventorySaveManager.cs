using System;
using UnityEngine;
using static Inventory;

public class InventorySaveManager : SaveManager
{
    [SerializeField] private Inventory inventory;

    public override SaveManagerState GetState()
    {
        SerializableInventory serializableInventory =
            inventory.GetAsSerializableInventory();

        var saveManagerState = new InventorySaveManagerState
        {
            SaveGuid = saveGuid,
            serializableInventory = serializableInventory
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        InventorySaveManagerState inventoryState =
            (InventorySaveManagerState)saveManagerState;

        inventory.SetUpFromSerializableInventory(inventoryState.serializableInventory);
    }

    [Serializable]
    public class InventorySaveManagerState : SaveManagerState
    {
        public SerializableInventory serializableInventory;
    }
}
