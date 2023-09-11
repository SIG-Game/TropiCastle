using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Throwable Item")]
public class ThrowableItemScriptableObject : ItemScriptableObject
{
    public int damage;
    public float knockback;
    public float speed;

    public override string GetAdditionalInfo() => $"\nThrowable\n" +
        $"Deals {damage} Damage\n{knockback} Knockback\n{speed} Speed";
}
