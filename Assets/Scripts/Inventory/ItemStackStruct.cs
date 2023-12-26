using System;
using Newtonsoft.Json;

// Experimental type to potentially replace the ItemStack class
[Serializable, JsonObject(MemberSerialization.OptIn)]
public struct ItemStackStruct
{
    public ItemScriptableObject ItemDefinition;

    [JsonProperty(Order = 1)] public int Amount;
    [JsonProperty(Order = 2)] public ItemInstanceProperties InstanceProperties;

    [JsonProperty(Order = 0)] public string ItemName => ItemDefinition.name;

    public ItemStackStruct(ItemScriptableObject itemDefinition, int amount,
        ItemInstanceProperties instanceProperties = null)
    {
        ItemDefinition = itemDefinition;
        Amount = amount;
        InstanceProperties = instanceProperties;
    }

    [JsonConstructor]
    public ItemStackStruct(string itemName, int amount,
        ItemInstanceProperties instanceProperties = null) :
            this(ItemScriptableObject.FromName(itemName), amount, instanceProperties)
    {
    }

    public ItemStackStruct(ItemStackStruct item) : this(item.ItemDefinition,
        item.Amount, item.InstanceProperties?.DeepCopy())
    {
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

    public ItemStack ToClassType() =>
        new ItemStack(ItemDefinition, Amount, InstanceProperties);

    public override string ToString() => $"{Amount} {ItemDefinition.DisplayName}";

    public static implicit operator ItemStackStruct(ItemStack itemStack) =>
        itemStack.ToStructType();
}
