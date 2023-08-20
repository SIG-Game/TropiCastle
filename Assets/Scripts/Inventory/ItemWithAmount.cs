using System;
using System.Collections.Generic;

[Serializable]
public class ItemWithAmount
{
    public ItemScriptableObject itemData;
    public int amount;
    public ItemInstanceProperties instanceProperties;

    private static readonly Dictionary<string, Type> itemNameToInstancePropertiesType =
        new Dictionary<string, Type>
    {
        { "Chest", typeof(ChestItemInstanceProperties) }
    };

    public ItemWithAmount(ItemScriptableObject itemData, int amount,
        ItemInstanceProperties instanceProperties = null)
    {
        this.itemData = itemData;
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

    public ItemWithAmount(ItemWithAmount item)
    {
        itemData = item.itemData;
        amount = item.amount;
        instanceProperties = item.instanceProperties?.DeepCopy();
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
            instanceProperties = (ItemInstanceProperties)Activator
                .CreateInstance(itemInstancePropertiesType);
        }
    }

    public string GetTooltipText()
    {
        if (TryGetDurabilityProperties(out int durability, out int initialDurability))
        {
            return $"{itemData.name}\nDurability: {durability} / {initialDurability}";
        }
        else
        {
            return itemData.GetTooltipText();
        }
    }

    public bool TryGetDurabilityProperties(out int durability, out int initialDurability)
    {
        if (instanceProperties is BreakableItemInstanceProperties breakableItemProperties &&
            itemData is BreakableItemScriptableObject breakableItemScriptableObject)
        {
            durability = breakableItemProperties.Durability;
            initialDurability = breakableItemScriptableObject.InitialDurability;

            return true;
        }
        else
        {
            durability = -1;
            initialDurability = -1;

            return false;
        }
    }

    public string GetAmountText() => amount > 1 ? amount.ToString() : string.Empty;
}
