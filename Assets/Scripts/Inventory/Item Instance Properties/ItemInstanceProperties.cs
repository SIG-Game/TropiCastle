using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

[Serializable]
public class ItemInstanceProperties
{
    public List<ItemInstanceProperty> PropertyList;

    public ItemInstanceProperties()
    {
        PropertyList = new List<ItemInstanceProperty>();
    }

    public string GetStringProperty(string name) =>
        PropertyList.Find(x => x.Name == name).Value;

    public int GetIntProperty(string name) =>
        int.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public void SetExistingProperty(string name, string value) =>
        PropertyList.Find(x => x.Name == name).Value = value;

    public void AddProperty(string name, string value) =>
        PropertyList.Add(new ItemInstanceProperty(name, value));

    public bool HasProperty(string name) =>
        PropertyList.Exists(x => x.Name == name);

    public virtual ItemInstanceProperties DeepCopy()
    {
        List<ItemInstanceProperty> propertyListDeepCopy = PropertyList
            .Select(x => new ItemInstanceProperty(x.Name, x.Value)).ToList();

        ItemInstanceProperties deepCopy =
            (ItemInstanceProperties)MemberwiseClone();

        deepCopy.PropertyList = propertyListDeepCopy;

        return deepCopy;
    }
}
