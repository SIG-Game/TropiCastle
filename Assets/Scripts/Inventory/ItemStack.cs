using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class ItemStack
{
    public ItemScriptableObject itemDefinition;

    [JsonProperty(Order = 1)] public int amount;
    [JsonProperty(Order = 2)] public PropertyCollection instanceProperties;

    [JsonProperty(Order = 0)] public string ItemName => itemDefinition.name;

    [JsonConstructor]
    public ItemStack(string itemName, int amount,
        PropertyCollection instanceProperties = null)
    {
        itemDefinition = ItemScriptableObject.FromName(itemName);
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

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
            instanceProperties = new PropertyCollection();

            int containerSize = itemDefinition.GetIntProperty("ContainerSize");
            instanceProperties.AddItemListProperty(containerSize);
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

    public override string ToString() => $"{amount} {itemDefinition.DisplayName}";
}
