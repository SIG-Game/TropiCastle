using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public int StackSize = defaultStackSize;
    public bool LockPlacementToGrid;
    public bool OneAtATimePlacement;
    public bool HasCustomColliderSize;
    public bool TriggerCollisionPickup;
    public Vector2 CustomColliderSize;
    public List<ItemProperty> Properties;

    private const int defaultStackSize = 99;

    public bool IsEmpty() => Name == "Empty";

    public string GetStringProperty(string name) =>
        Properties.Find(x => x.Name == name).Value;

    public float GetFloatProperty(string name) =>
        float.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public int GetIntProperty(string name) =>
        int.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public bool HasProperty(string name) =>
        Properties.Exists(x => x.Name == name);

    public string GetTooltipText(bool includeInitialDurability = true)
    {
        string tooltipText;

        if (IsEmpty())
        {
            tooltipText = string.Empty;
        }
        else
        {
            tooltipText = string.Concat(Name, GetAdditionalInfo());

            if (includeInitialDurability && HasProperty("InitialDurability"))
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
