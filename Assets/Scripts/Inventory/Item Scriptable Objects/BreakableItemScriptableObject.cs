using UnityEngine;

[CreateAssetMenu(menuName = "Breakable Item")]
public class BreakableItemScriptableObject : ItemScriptableObject
{
    public int InitialDurability;

    public override string GetAdditionalInfo() => $"\nDurability: {InitialDurability}";
}
