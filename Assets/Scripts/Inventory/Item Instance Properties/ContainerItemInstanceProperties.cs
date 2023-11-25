using System;
using System.Collections.Generic;
using static Inventory;

[Serializable]
public class ContainerItemInstanceProperties : PropertyCollection
{
    public SerializableInventory SerializableInventory;

    public ContainerItemInstanceProperties(int containerSize)
    {
        List<SerializableInventoryItem> containerItemList =
            new List<SerializableInventoryItem>(containerSize);

        for (int i = 0; i < containerSize; ++i)
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
        int containerSize = SerializableInventory.SerializableItemList.Count;

        List<SerializableInventoryItem> containerItemListDeepCopy =
            new List<SerializableInventoryItem>(containerSize);

        for (int i = 0; i < containerSize; ++i)
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
