using System;

[Serializable]
public class ItemInstanceProperty
{
    public string Name;
    public string Value;

    public ItemInstanceProperty(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
