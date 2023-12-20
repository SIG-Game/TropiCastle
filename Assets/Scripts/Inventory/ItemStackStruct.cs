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

    public string GetAmountText() => Amount > 1 ? Amount.ToString() : string.Empty;

    public ItemStack ToClassType() =>
        new ItemStack(ItemDefinition, Amount, InstanceProperties);

    public override string ToString() => $"{Amount} {ItemDefinition.DisplayName}";

    public static implicit operator ItemStackStruct(ItemStack itemStack) =>
        itemStack.ToStructType();
}
