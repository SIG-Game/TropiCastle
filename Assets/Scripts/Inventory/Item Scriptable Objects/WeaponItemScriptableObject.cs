using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Item")]
public class WeaponItemScriptableObject : ItemScriptableObject
{
    public Sprite weaponSprite;
    public int damage;
    public float knockback;
}
