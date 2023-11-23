using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string DisplayName;
    public Sprite Sprite;
    public int StackSize = defaultStackSize;
    public bool LockPlacementToGrid;
    public bool OneAtATimePlacement;
    public bool TriggerCollisionPickup;
    public bool HasCustomColliderSize;
    public Vector2 CustomColliderSize;
    public PropertyCollection Properties;

    private const int defaultStackSize = 99;

    public bool IsEmpty() => name == "Empty";

    public string GetTooltipText(bool includeInitialDurability = true)
    {
        string tooltipText;

        if (IsEmpty())
        {
            tooltipText = string.Empty;
        }
        else
        {
            tooltipText = DisplayName;

            if (HasProperty("AttackType"))
            {
                tooltipText += $"\n{GetStringProperty("AttackType")} Attack" +
                    $"\nDeals {GetIntProperty("Damage")} Damage" +
                    $"\n{GetFloatProperty("Knockback")} Knockback" +
                    $"\n{GetFloatProperty("AttackSpeed")} Attack Speed";
            }

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

    public string GetStringProperty(string name) => Properties.GetStringProperty(name);
    public float GetFloatProperty(string name) => Properties.GetFloatProperty(name);
    public int GetIntProperty(string name) => Properties.GetIntProperty(name);
    public bool HasProperty(string name) => Properties.HasProperty(name);

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
