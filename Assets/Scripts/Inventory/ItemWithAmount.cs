using System;
using System.Collections.Generic;

[Serializable]
public class ItemWithAmount
{
    public ItemScriptableObject itemData;
    public int amount;
    public object instanceProperties;

    private static readonly Dictionary<string, Type> itemNameToInstancePropertiesType =
        new Dictionary<string, Type>
    {
        { "Chest", typeof(ChestItemInstanceProperties) },
        { "Fishing Rod", typeof(FishingRodItemInstanceProperties) }
    };

    public ItemWithAmount(ItemScriptableObject itemData, int amount,
        object instanceProperties = null)
    {
        this.itemData = itemData;
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

    public void InitializeItemInstanceProperties()
    {
        if (itemNameToInstancePropertiesType.TryGetValue(itemData.name,
            out Type itemInstancePropertiesType))
        {
            instanceProperties = Activator.CreateInstance(itemInstancePropertiesType);
        }
    }
}
