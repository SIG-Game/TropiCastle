using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

[Serializable]
public class PropertyCollection
{
    public List<Property> PropertyList;

    // TODO: Store all item instance properties in a Dictionary
    public Dictionary<string, object> PropertyDictionary;

    public PropertyCollection()
    {
        PropertyList = new List<Property>();
        PropertyDictionary = new Dictionary<string, object>();
    }

    public PropertyCollection(List<Property> propertyList,
        Dictionary<string, object> propertyDictionary)
    {
        PropertyList = propertyList;
        PropertyDictionary = propertyDictionary;
    }

    public string GetStringProperty(string name) =>
        PropertyList.Find(x => x.Name == name).Value;

    public float GetFloatProperty(string name) =>
        float.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public int GetIntProperty(string name) =>
        int.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public bool GetBoolProperty(string name) =>
        bool.Parse(GetStringProperty(name));

    public void SetExistingProperty(string name, string value) =>
        PropertyList.Find(x => x.Name == name).Value = value;

    public void AddProperty(string name, string value) =>
        PropertyList.Add(new Property(name, value));

    public bool HasProperty(string name) =>
        PropertyList.Exists(x => x.Name == name);

    // TODO: Do not store properties in two variables in this class
    public int GetDictionaryIntProperty(string name) =>
        Convert.ToInt32(PropertyDictionary[name]);

    public void SetDictionaryProperty(string name, object value) =>
        PropertyDictionary[name] = value;

    public bool HasDictionaryProperty(string name) =>
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

    public virtual PropertyCollection DeepCopy()
    {
        List<Property> propertyListDeepCopy = PropertyList
            .Select(x => new Property(x.Name, x.Value)).ToList();

        PropertyCollection deepCopy =
            (PropertyCollection)MemberwiseClone();

        deepCopy.PropertyList = propertyListDeepCopy;

        if (PropertyDictionary.ContainsKey("ItemList"))
        {
            var itemList = (List<ItemStack>)PropertyDictionary["ItemList"];

            var itemListDeepCopy = new List<ItemStack>(itemList.Count);

            for (int i = 0; i < itemList.Count; ++i)
            {
                itemListDeepCopy.Add(new ItemStack(itemList[i]));
            }

            deepCopy.PropertyDictionary.Add("ItemList", itemList);
        }

        return deepCopy;
    }
}
