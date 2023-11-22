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
            SerializedInventory = JsonUtility.ToJson(serializableInventory)
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        InventorySaveManagerState inventoryState =
            (InventorySaveManagerState)saveManagerState;

        SerializableInventory serializableInventory = JsonUtility
            .FromJson<SerializableInventory>(inventoryState.SerializedInventory);

        inventory.SetUpFromSerializableInventory(serializableInventory);
    }

    [Serializable]
    public class InventorySaveManagerState : SaveManagerState
    {
        public string SerializedInventory;
    }
}
