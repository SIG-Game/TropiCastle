using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class ItemStack
{
    public ItemScriptableObject itemDefinition;

    [JsonProperty(Order = 1)] public int amount;
    [JsonProperty(Order = 2)] public ItemInstanceProperties instanceProperties;

    [JsonProperty(Order = 0)] public string ItemName => itemDefinition.name;

    public ItemStack(ItemScriptableObject itemDefinition, int amount,
        ItemInstanceProperties instanceProperties = null)
    {
        this.itemDefinition = itemDefinition;
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

    [JsonConstructor]
    public ItemStack(string itemName, int amount,
        ItemInstanceProperties instanceProperties = null) :
            this(ItemScriptableObject.FromName(itemName), amount, instanceProperties)
    {
    }

    public ItemStack(ItemStack item) : this(item.itemDefinition, item.amount,
        item.instanceProperties?.DeepCopy())
    {
    }

    public void InitializeItemInstanceProperties()
    {
        List<Property> defaultInstancePropertyList =
            itemDefinition.DefaultInstanceProperties.PropertyList;

        if (itemDefinition.HasProperty("ContainerSize"))
        {
            instanceProperties = new ItemInstanceProperties();

            int containerSize = itemDefinition.GetIntProperty("ContainerSize");
            instanceProperties.AddItemListProperty(containerSize);
        }
        else if (defaultInstancePropertyList.Count != 0)
        {
            instanceProperties = new ItemInstanceProperties();
        }

        foreach (var property in defaultInstancePropertyList)
        {
            instanceProperties.SetProperty(property.Name, property.Value);
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

    public ItemStackStruct ToStructType() => new ItemStackStruct(itemDefinition, amount);

    public override string ToString() => $"{amount} {itemDefinition.DisplayName}";
}
