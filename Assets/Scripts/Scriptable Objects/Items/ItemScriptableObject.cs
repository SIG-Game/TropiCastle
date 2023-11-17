using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int stackSize = defaultStackSize;
    public bool lockPlacementToGrid;
    public bool oneAtATimePlacement;
    public bool hasCustomColliderSize;
    public bool triggerCollisionPickup;
    public Vector2 customColliderSize;
    public List<ItemProperty> properties;

    private const int defaultStackSize = 99;

    public bool IsEmpty() => name == "Empty";

    public string GetStringProperty(string name) =>
        properties.Find(x => x.Name == name).Value;

    public float GetFloatProperty(string name) =>
        float.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public int GetIntProperty(string name) =>
        int.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public bool HasProperty(string name) =>
        properties.Exists(x => x.Name == name);

    public string GetTooltipText()
    {
        string tooltipText;

        if (IsEmpty())
        {
            tooltipText = string.Empty;
        }
        else
        {
            tooltipText = string.Concat(name, GetAdditionalInfo());

            if (HasProperty("InitialDurability"))
            {
                tooltipText += $"\nDurability: {GetIntProperty("InitialDurability")}";
            }

            if (HasProperty("HealAmount"))
            {
                tooltipText += $"\nHeals {GetIntProperty("HealAmount")} Health";
            }
        }

        return tooltipText;
    }

    public virtual string GetAdditionalInfo() => string.Empty;

    public static ItemScriptableObject FromName(string name)
    {
        string itemScriptableObjectKey = $"Items/{name}.asset";

        var itemLoadHandle = Addressables
            .LoadAssetAsync<ItemScriptableObject>(itemScriptableObjectKey);

        ItemScriptableObject item = itemLoadHandle.WaitForCompletion();

        if (itemLoadHandle.IsValid())
        {
            Addressables.Release(itemLoadHandle);
        }

        return item;
    }
}
