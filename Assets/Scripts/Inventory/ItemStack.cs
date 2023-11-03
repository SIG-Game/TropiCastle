using System;
using System.Collections.Generic;

[Serializable]
public class ItemStack
{
    public ItemScriptableObject itemDefinition;
    public int amount;
    public ItemInstanceProperties instanceProperties;

    private static readonly Dictionary<string, Type> itemNameToInstancePropertiesType =
        new Dictionary<string, Type>
    {
        { "Campfire", typeof(CampfireItemInstanceProperties) },
        { "Chest", typeof(ChestItemInstanceProperties) }
    };

    public ItemStack(ItemScriptableObject itemDefinition, int amount,
        ItemInstanceProperties instanceProperties = null)
    {
        this.itemDefinition = itemDefinition;
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

    public ItemStack(ItemStack item)
    {
        itemDefinition = item.itemDefinition;
        amount = item.amount;
        instanceProperties = item.instanceProperties?.DeepCopy();
    }

    public void InitializeItemInstanceProperties()
    {
        if (itemDefinition is BreakableItemScriptableObject breakableItemScriptableObject)
        {
            instanceProperties = new BreakableItemInstanceProperties(
                breakableItemScriptableObject.InitialDurability);
        }
        else if (itemNameToInstancePropertiesType.TryGetValue(itemDefinition.name,
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
            return $"{itemDefinition.name}\nDurability: {durability} / {initialDurability}";
        }
        else
        {
            return itemDefinition.GetTooltipText();
        }
    }

    public bool TryGetDurabilityProperties(out int durability, out int initialDurability)
    {
        if (instanceProperties is BreakableItemInstanceProperties breakableItemProperties &&
            itemDefinition is BreakableItemScriptableObject breakableItemScriptableObject)
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

    public ItemStack GetCopyWithAmount(int amount) =>
        new ItemStack(itemDefinition, amount, instanceProperties);

    public string GetAmountText() => amount > 1 ? amount.ToString() : string.Empty;
}
