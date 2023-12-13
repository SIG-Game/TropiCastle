using System;
using System.Collections.Generic;
using System.Linq;

public class ItemInstanceProperties
{
    public Dictionary<string, object> PropertyDictionary;

    public ItemInstanceProperties()
    {
        PropertyDictionary = new Dictionary<string, object>();
    }

    public ItemInstanceProperties(Dictionary<string, object> propertyDictionary)
    {
        PropertyDictionary = propertyDictionary;
    }

    public int GetIntProperty(string name) =>
        Convert.ToInt32(PropertyDictionary[name]);

    public float GetFloatProperty(string name) =>
        Convert.ToSingle(PropertyDictionary[name]);

    public void SetProperty(string name, object value) =>
        PropertyDictionary[name] = value;

    public bool HasProperty(string name) =>
        PropertyDictionary.ContainsKey(name);

    public void AddItemListProperty(int itemListCount)
    {
        var itemList = new List<ItemStack>(itemListCount);

        for (int i = 0; i < itemListCount; ++i)
        {
            itemList.Add(new ItemStack("Empty", 0));
        }

        PropertyDictionary["ItemList"] = itemList;
    }

    public void UpdateItemListProperty(Inventory inventory)
    {
        PropertyDictionary["ItemList"] = inventory.GetItemList();
    }

    public virtual ItemInstanceProperties DeepCopy()
    {
        ItemInstanceProperties deepCopy =
            (ItemInstanceProperties)MemberwiseClone();

        deepCopy.PropertyDictionary = 
            PropertyDictionary.ToDictionary(x => x.Key, x => x.Value);

        if (PropertyDictionary.ContainsKey("ItemList"))
        {
            var itemList = (List<ItemStack>)PropertyDictionary["ItemList"];

            var itemListDeepCopy = new List<ItemStack>(itemList.Count);

            for (int i = 0; i < itemList.Count; ++i)
            {
                itemListDeepCopy.Add(new ItemStack(itemList[i]));
            }

            deepCopy.PropertyDictionary["ItemList"] = itemListDeepCopy;
        }

        return deepCopy;
    }
}
