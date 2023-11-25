using System;
using System.Collections.Generic;

[Serializable]
public class ItemStack
{
    public ItemScriptableObject itemDefinition;
    public int amount;
    public PropertyCollection instanceProperties;

    public ItemStack(ItemScriptableObject itemDefinition, int amount,
        PropertyCollection instanceProperties = null)
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
        List<Property> defaultInstancePropertyList =
            itemDefinition.DefaultInstanceProperties.PropertyList;

        if (itemDefinition.HasProperty("ContainerSize"))
        {
            int containerSize = itemDefinition.GetIntProperty("ContainerSize");

            instanceProperties = new ContainerItemInstanceProperties(containerSize);
        }
        else if (defaultInstancePropertyList.Count != 0)
        {
            instanceProperties = new PropertyCollection();
        }

        foreach (var property in defaultInstancePropertyList)
        {
            instanceProperties.AddProperty(property.Name, property.Value);
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
            itemDefinition.DefaultInstanceProperties.HasProperty("Durability"))
        {
            durability = instanceProperties.GetIntProperty("Durability");
            initialDurability =
                itemDefinition.DefaultInstanceProperties.GetIntProperty("Durability");

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
