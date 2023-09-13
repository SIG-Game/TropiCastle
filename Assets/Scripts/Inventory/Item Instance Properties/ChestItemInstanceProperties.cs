using System;
using System.Collections.Generic;
using static Inventory;

[Serializable]
public class ChestItemInstanceProperties : ItemInstanceProperties
{
    public SavableInventoryState SerializableInventory;

    public const int ChestInventorySize = 10;

    public ChestItemInstanceProperties()
    {
        List<SerializableInventoryItem> chestItemList =
            new List<SerializableInventoryItem>(ChestInventorySize);

        for (int i = 0; i < ChestInventorySize; ++i)
        {
            chestItemList.Add(new SerializableInventoryItem
            {
                ItemName = "Empty",
                Amount = 0
            });
        }

        SerializableInventory = new SavableInventoryState
        {
            SerializableItemList = chestItemList
        };
    }

    public void UpdateSerializableInventory(Inventory inventory)
    {
        SerializableInventory =
            (SavableInventoryState)inventory.GetSavableState();
    }

    public override ItemInstanceProperties DeepCopy()
    {
        List<SerializableInventoryItem> chestItemListDeepCopy =
            new List<SerializableInventoryItem>(ChestInventorySize);

        for (int i = 0; i < ChestInventorySize; ++i)
        {
            chestItemListDeepCopy.Add(new SerializableInventoryItem(
                SerializableInventory.SerializableItemList[i]));
        }

        ChestItemInstanceProperties deepCopy =
            (ChestItemInstanceProperties)base.DeepCopy();

        deepCopy.SerializableInventory = new SavableInventoryState
        {
            SerializableItemList = chestItemListDeepCopy
        };

        return deepCopy;
    }
}
