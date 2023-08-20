using System;

[Serializable]
public class BreakableItemInstanceProperties : ItemInstanceProperties
{
    public int Durability;

    public BreakableItemInstanceProperties(int durability)
    {
        Durability = durability;
    }
}
