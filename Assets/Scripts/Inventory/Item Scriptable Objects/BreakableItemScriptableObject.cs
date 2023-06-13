using UnityEngine;

[CreateAssetMenu(menuName = "Breakable Item")]
public class BreakableItemScriptableObject : ItemScriptableObject
{
    public int InitialDurability;

    public override string GetTooltipText() => $"{name}\nDurability: {InitialDurability}";
}
