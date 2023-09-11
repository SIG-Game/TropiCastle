using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Healing Item")]
public class HealingItemScriptableObject : ItemScriptableObject
{
    public int healAmount;

    public override string GetAdditionalInfo() => $"\nHeals {healAmount} Health";
}
