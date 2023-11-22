using System;

[Serializable]
public class Property
{
    public string Name;
    public string Value;

    public Property(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
