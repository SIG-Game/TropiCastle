using System;

[AttributeUsage(AttributeTargets.Field)]
public class OptionalInjectAttribute : InjectAttribute
{
    public OptionalInjectAttribute() {}
    public OptionalInjectAttribute(string name) : base(name) {}
}
