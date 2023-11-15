using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Weapon Item")]
public class WeaponItemScriptableObject : ItemScriptableObject
{
    public Sprite weaponSprite;
    public List<ItemProperty> properties;

    public string GetStringProperty(string name) =>
        properties.Find(x => x.Name == name).Value;

    public float GetFloatProperty(string name) =>
        float.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public int GetIntProperty(string name) =>
        int.Parse(GetStringProperty(name), CultureInfo.InvariantCulture);

    public override string GetAdditionalInfo() =>
        $"\n{GetStringProperty("AttackType")} Attack\n" +
        $"Deals {GetIntProperty("Damage")} Damage\n" +
        $"{GetFloatProperty("Knockback")} Knockback\n" +
        $"{GetFloatProperty("AttackSpeed")} Attack Speed";
}
