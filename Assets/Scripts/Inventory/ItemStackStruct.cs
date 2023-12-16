using System;

// Experimental type to potentially replace the ItemStack class
[Serializable]
public struct ItemStackStruct
{
    public ItemScriptableObject ItemDefinition;
    public int Amount;

    public ItemStackStruct(ItemScriptableObject itemDefinition, int amount)
    {
        ItemDefinition = itemDefinition;
        Amount = amount;
    }

    public ItemStack ToClassType() => new ItemStack(ItemDefinition, Amount);
}
