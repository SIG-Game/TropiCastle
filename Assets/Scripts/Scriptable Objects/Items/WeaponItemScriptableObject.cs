using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Weapon Item")]
public class WeaponItemScriptableObject : ItemScriptableObject
{
    public Sprite weaponSprite;
    public int damage;
    public float knockback;
    public float attackSpeed;
    public List<ItemProperty> properties;

    public string GetProperty(string name) => properties.Find(x => x.Name == name).Value;

    public override string GetAdditionalInfo() => $"\n{GetProperty("AttackType")} Attack\n" +
        $"Deals {damage} Damage\n{knockback} Knockback\n{attackSpeed} Attack Speed";
}
