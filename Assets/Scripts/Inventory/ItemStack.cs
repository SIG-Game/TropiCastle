using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public struct ItemStack
{
    public ItemScriptableObject ItemDefinition;

    [JsonProperty(Order = 1)] public int Amount;
    [JsonProperty(Order = 2)] public ItemInstanceProperties InstanceProperties;

    [JsonProperty(Order = 0)] public string ItemName => ItemDefinition.name;

    public ItemStack(ItemScriptableObject itemDefinition, int amount,
        ItemInstanceProperties instanceProperties = null)
    {
        ItemDefinition = itemDefinition;
        Amount = amount;
        InstanceProperties = instanceProperties;
    }

    [JsonConstructor]
    public ItemStack(string itemName, int amount,
        ItemInstanceProperties instanceProperties = null) :
            this(ItemScriptableObject.FromName(itemName), amount, instanceProperties)
    {
    }

    public ItemStack(ItemStack item) : this(item.ItemDefinition,
        item.Amount, item.InstanceProperties?.DeepCopy())
    {
    }

    public void InitializeItemInstanceProperties()
    {
        List<Property> defaultInstancePropertyList =
            ItemDefinition.DefaultInstanceProperties.PropertyList;

        if (ItemDefinition.HasProperty("ContainerSize"))
        {
            InstanceProperties = new ItemInstanceProperties();

            int containerSize = ItemDefinition.GetIntProperty("ContainerSize");
            InstanceProperties.AddItemListProperty(containerSize);
        }
        else if (defaultInstancePropertyList.Count != 0)
        {
            InstanceProperties = new ItemInstanceProperties();
        }

        foreach (var property in defaultInstancePropertyList)
        {
            InstanceProperties.SetProperty(property.Name, property.Value);
        }
    }

    public string GetTooltipText()
    {
        string tooltipText = ItemDefinition.GetTooltipText(includeInitialDurability: false);

        if (TryGetDurabilityProperties(out int durability, out int initialDurability))
        {
            tooltipText += $"\nDurability: {durability} / {initialDurability}";
        }

        return tooltipText;
    }

    public bool TryGetDurabilityProperties(out int durability, out int initialDurability)
    {
        if (InstanceProperties != null &&
            InstanceProperties.HasProperty("Durability") &&
            ItemDefinition.DefaultInstanceProperties.HasProperty("Durability"))
        {
            durability = InstanceProperties.GetIntProperty("Durability");
            initialDurability =
                ItemDefinition.DefaultInstanceProperties.GetIntProperty("Durability");

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
        new ItemStack(ItemDefinition, amount, InstanceProperties);

    public string GetAmountText() => Amount > 1 ? Amount.ToString() : string.Empty;

    public override string ToString() => $"{Amount} {ItemDefinition.DisplayName}";
}
