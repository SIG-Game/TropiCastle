using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

[Serializable]
public class PropertyCollection
{
    public List<Property> PropertyList;

    public PropertyCollection()
    {
        PropertyList = new List<Property>();
    }

    public PropertyCollection(List<Property> propertyList)
    {
        PropertyList = propertyList;
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

    public virtual PropertyCollection DeepCopy()
    {
        List<Property> propertyListDeepCopy = PropertyList
            .Select(x => new Property(x.Name, x.Value)).ToList();

        PropertyCollection deepCopy =
            (PropertyCollection)MemberwiseClone();

        deepCopy.PropertyList = propertyListDeepCopy;

        return deepCopy;
    }
}
