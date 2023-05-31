using System;

[Serializable]
public class BreakableItemInstanceProperties
{
    public int Durability;

    public BreakableItemInstanceProperties(int durability)
    {
        Durability = durability;
    }
}
