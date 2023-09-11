using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Breakable Item")]
public class BreakableItemScriptableObject : ItemScriptableObject
{
    public int InitialDurability;

    public override string GetAdditionalInfo() => $"\nDurability: {InitialDurability}";
}
