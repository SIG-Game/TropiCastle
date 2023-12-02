using System;
using System.Collections.Generic;

[Serializable]
public class ContainerItemInstanceProperties : PropertyCollection
{
    // TODO: Store all item instance properties in a Dictionary
    public Dictionary<string, object> PropertyDictionary;

    public ContainerItemInstanceProperties(int containerSize)
    {
        PropertyDictionary = new Dictionary<string, object>();

        List<SerializableItem> containerItemList =
            new List<SerializableItem>(containerSize);

        for (int i = 0; i < containerSize; ++i)
        {
            containerItemList.Add(new SerializableItem
            {
                ItemName = "Empty",
                Amount = 0
            });
        }

        PropertyDictionary["ItemList"] = containerItemList;
    }

    public void UpdateSerializableItemList(Inventory inventory)
    {
        PropertyDictionary["ItemList"] =
            inventory.GetAsSerializableItemList();
    }

    public override PropertyCollection DeepCopy()
    {
        var containerItemList = (List<SerializableItem>)
            PropertyDictionary["ItemList"];

        List<SerializableItem> containerItemListDeepCopy =
            new List<SerializableItem>(containerItemList.Count);

        for (int i = 0; i < containerItemList.Count; ++i)
        {
            containerItemListDeepCopy.Add(
                new SerializableItem(containerItemList[i]));
        }

        ContainerItemInstanceProperties deepCopy =
            (ContainerItemInstanceProperties)base.DeepCopy();

        deepCopy.PropertyDictionary = new Dictionary<string, object>
        {
            { "ItemList", containerItemListDeepCopy }
        };

        return deepCopy;
    }
}
