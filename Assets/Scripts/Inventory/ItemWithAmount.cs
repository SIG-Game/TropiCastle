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
        { "Chest", typeof(ChestItemInstanceProperties) }
    };

    public ItemWithAmount(ItemScriptableObject itemData, int amount,
        object instanceProperties = null)
    {
        this.itemData = itemData;
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

    public ItemWithAmount(ItemWithAmount item)
    {
        itemData = item.itemData;
        amount = item.amount;

        // TODO: Use a deep copy of item.instanceProperties
        instanceProperties = item.instanceProperties;
    }

    public void InitializeItemInstanceProperties()
    {
        if (itemData is BreakableItemScriptableObject breakableItemScriptableObject)
        {
            instanceProperties = new BreakableItemInstanceProperties(
                breakableItemScriptableObject.InitialDurability);
        }
        else if (itemNameToInstancePropertiesType.TryGetValue(itemData.name,
            out Type itemInstancePropertiesType))
        {
            instanceProperties = Activator.CreateInstance(itemInstancePropertiesType);
        }
    }

    public string GetTooltipText()
    {
        if (instanceProperties is BreakableItemInstanceProperties breakableItemProperties &&
            itemData is BreakableItemScriptableObject breakableItemScriptableObject)
        {
            return $"{itemData.name}\nDurability: {breakableItemProperties.Durability} " +
                $"/ {breakableItemScriptableObject.InitialDurability}";
        }
        else
        {
            return itemData.GetTooltipText();
        }
    }

    public string GetAmountText() => amount > 1 ? amount.ToString() : string.Empty;
}
