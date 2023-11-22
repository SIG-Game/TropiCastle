using System;

[Serializable]
public class BreakableItemInstanceProperties : PropertyCollection
{
    public int Durability;

    public BreakableItemInstanceProperties(int durability)
    {
        Durability = durability;
    }
}
