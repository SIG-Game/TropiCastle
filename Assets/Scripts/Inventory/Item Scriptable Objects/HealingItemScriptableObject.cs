using UnityEngine;

[CreateAssetMenu(menuName = "Healing Item")]
public class HealingItemScriptableObject : ItemScriptableObject
{
    public int healAmount;

    public override string GetTooltipText() => $"{name}\nHeals {healAmount} Health";
}
