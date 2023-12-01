using System;
using System.Collections.Generic;
using static Inventory;

[Serializable]
public class ContainerItemInstanceProperties : PropertyCollection
{
    // TODO: Store all item instance properties in a Dictionary
    public Dictionary<string, object> PropertyDictionary;

    public ContainerItemInstanceProperties(int containerSize)
    {
        PropertyDictionary = new Dictionary<string, object>();

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

        PropertyDictionary["SerializableInventory"] =
            new SerializableInventory
        {
            SerializableItemList = containerItemList
        };
    }

    public void UpdateSerializableInventory(Inventory inventory)
    {
        PropertyDictionary["SerializableInventory"] =
            inventory.GetAsSerializableInventory();
    }

    public override PropertyCollection DeepCopy()
    {
        var serializableInventory = (SerializableInventory)
            PropertyDictionary["SerializableInventory"];

        int containerSize = serializableInventory.SerializableItemList.Count;

        List<SerializableInventoryItem> containerItemListDeepCopy =
            new List<SerializableInventoryItem>(containerSize);

        for (int i = 0; i < containerSize; ++i)
        {
            containerItemListDeepCopy.Add(new SerializableInventoryItem(
                serializableInventory.SerializableItemList[i]));
        }

        ContainerItemInstanceProperties deepCopy =
            (ContainerItemInstanceProperties)base.DeepCopy();

        var serializableInventoryDeepCopy = new SerializableInventory
        {
            SerializableItemList = containerItemListDeepCopy
        };

        deepCopy.PropertyDictionary = new Dictionary<string, object>
        {
            { "SerializableInventory", serializableInventoryDeepCopy }
        };

        return deepCopy;
    }
}
