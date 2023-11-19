using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Weapon Item")]
public class WeaponItemScriptableObject : ItemScriptableObject
{
    public Sprite WeaponSprite;

    public override string GetAdditionalInfo() =>
        $"\n{GetStringProperty("AttackType")} Attack\n" +
        $"Deals {GetIntProperty("Damage")} Damage\n" +
        $"{GetFloatProperty("Knockback")} Knockback\n" +
        $"{GetFloatProperty("AttackSpeed")} Attack Speed";
}
