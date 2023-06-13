using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Item")]
public class WeaponItemScriptableObject : ItemScriptableObject
{
    public Sprite weaponSprite;
    public WeaponAttackType attackType;
    public int damage;
    public float knockback;
    public float attackSpeed;

    public override string GetTooltipText() => $"{name}\n{attackType} Attack\n" +
        $"Deals {damage} Damage\n{knockback} Knockback\n{attackSpeed} Attack Speed";
}
