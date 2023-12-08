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

        var containerItemList = new List<ItemStack>(containerSize);

        for (int i = 0; i < containerSize; ++i)
        {
            containerItemList.Add(new ItemStack("Empty", 0));
        }

        PropertyDictionary["ItemList"] = containerItemList;
    }

    public void UpdateItemListProperty(Inventory inventory)
    {
        PropertyDictionary["ItemList"] = inventory.GetItemList();
    }

    public override PropertyCollection DeepCopy()
    {
        var itemList = (List<ItemStack>)PropertyDictionary["ItemList"];

        var itemListDeepCopy = new List<ItemStack>(itemList.Count);

        for (int i = 0; i < itemList.Count; ++i)
        {
            itemListDeepCopy.Add(new ItemStack(itemList[i]));
        }

        ContainerItemInstanceProperties deepCopy =
            (ContainerItemInstanceProperties)base.DeepCopy();

        deepCopy.PropertyDictionary = new Dictionary<string, object>
        {
            { "ItemList", itemListDeepCopy }
        };

        return deepCopy;
    }
}
