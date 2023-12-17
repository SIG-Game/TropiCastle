using System;

// Experimental type to potentially replace the ItemStack class
[Serializable]
public struct ItemStackStruct
{
    public ItemScriptableObject ItemDefinition;
    public int Amount;
    public ItemInstanceProperties InstanceProperties;

    public ItemStackStruct(ItemScriptableObject itemDefinition, int amount,
        ItemInstanceProperties instanceProperties = null)
    {
        ItemDefinition = itemDefinition;
        Amount = amount;
        InstanceProperties = instanceProperties;
    }

    public ItemStack ToClassType() =>
        new ItemStack(ItemDefinition, Amount, InstanceProperties);
}
