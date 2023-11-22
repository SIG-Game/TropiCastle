using System;
using System.Collections.Generic;
using static Inventory;

[Serializable]
public abstract class ContainerItemInstanceProperties : PropertyCollection
{
    public SerializableInventory SerializableInventory;

    public abstract int InventorySize { get; }

    public ContainerItemInstanceProperties()
    {
        List<SerializableInventoryItem> containerItemList =
            new List<SerializableInventoryItem>(InventorySize);

        for (int i = 0; i < InventorySize; ++i)
        {
            containerItemList.Add(new SerializableInventoryItem
            {
                ItemName = "Empty",
                Amount = 0
            });
        }

        SerializableInventory = new SerializableInventory
        {
            SerializableItemList = containerItemList
        };
    }

    public void UpdateSerializableInventory(Inventory inventory)
    {
        SerializableInventory = inventory.GetAsSerializableInventory();
    }

    public override PropertyCollection DeepCopy()
    {
        List<SerializableInventoryItem> containerItemListDeepCopy =
            new List<SerializableInventoryItem>(InventorySize);

        for (int i = 0; i < InventorySize; ++i)
        {
            containerItemListDeepCopy.Add(new SerializableInventoryItem(
                SerializableInventory.SerializableItemList[i]));
        }

        ContainerItemInstanceProperties deepCopy =
            (ContainerItemInstanceProperties)base.DeepCopy();

        deepCopy.SerializableInventory = new SerializableInventory
        {
            SerializableItemList = containerItemListDeepCopy
        };

        return deepCopy;
    }
}
