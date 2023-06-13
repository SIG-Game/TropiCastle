using UnityEngine;

[CreateAssetMenu(menuName = "Throwable Item")]
public class ThrowableItemScriptableObject : ItemScriptableObject
{
    public int damage;

    public override string GetTooltipText() => $"{name}\nThrowable\nDeals {damage} Damage";
}
