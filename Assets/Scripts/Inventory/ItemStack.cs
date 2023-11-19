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
        if (itemDefinition.HasProperty("InitialDurability"))
        {
            instanceProperties = new ItemInstanceProperties();

            instanceProperties.AddProperty("Durability",
                itemDefinition.GetStringProperty("InitialDurability"));
        }
        else if (itemNameToInstancePropertiesType.TryGetValue(itemDefinition.Name,
            out Type itemInstancePropertiesType))
        {
            instanceProperties = (ItemInstanceProperties)Activator
                .CreateInstance(itemInstancePropertiesType);
        }
    }

    public string GetTooltipText()
    {
        string tooltipText = itemDefinition.GetTooltipText(includeInitialDurability: false);

        if (TryGetDurabilityProperties(out int durability, out int initialDurability))
        {
            tooltipText += $"\nDurability: {durability} / {initialDurability}";
        }

        return tooltipText;
    }

    public bool TryGetDurabilityProperties(out int durability, out int initialDurability)
    {
        if (instanceProperties != null &&
            instanceProperties.HasProperty("Durability") &&
            itemDefinition.HasProperty("InitialDurability"))
        {
            durability = instanceProperties.GetIntProperty("Durability");
            initialDurability = itemDefinition.GetIntProperty("InitialDurability");

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
