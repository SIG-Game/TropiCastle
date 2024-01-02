using System;

[AttributeUsage(AttributeTargets.Field)]
public class InjectAttribute : Attribute
{
    public string Name { get; }

    public InjectAttribute() {}
    public InjectAttribute(string name) => Name = name;
}
