using System;
using UnityEngine;
using static Inventory;

public class InventorySaveManager : SaveManager
{
    [SerializeField] private Inventory inventory;

    public override SavableState GetSavableState()
    {
        SerializableInventory serializableInventory =
            inventory.GetAsSerializableInventory();

        var savableState = new SavableInventoryState
        {
            SaveGuid = saveGuid,
            serializableInventory = serializableInventory
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
    {
        SavableInventoryState inventoryState = (SavableInventoryState)savableState;

        inventory.SetUpFromSerializableInventory(inventoryState.serializableInventory);
    }

    [Serializable]
    public class SavableInventoryState : SavableState
    {
        public SerializableInventory serializableInventory;

        public override Type GetSavableClassType() =>
            typeof(InventorySaveManager);
    }
}
