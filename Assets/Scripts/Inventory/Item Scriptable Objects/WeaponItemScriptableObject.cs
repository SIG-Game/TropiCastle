using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Weapon Item")]
public class WeaponItemScriptableObject : ItemScriptableObject
{
    public Sprite weaponSprite;
    public WeaponAttackType attackType;
    public int damage;
    public float knockback;
    public float attackSpeed;

    public override string GetAdditionalInfo() => $"\n{attackType} Attack\n" +
        $"Deals {damage} Damage\n{knockback} Knockback\n{attackSpeed} Attack Speed";
}
