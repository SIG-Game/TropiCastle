using UnityEngine;

[CreateAssetMenu(menuName = "Throwable Item")]
public class ThrowableItemScriptableObject : ItemScriptableObject
{
    public int damage;
    public float knockback;
    public float speed;

    public override string GetTooltipText() => $"{name}\nThrowable\n" +
        $"Deals {damage} Damage\n{knockback} Knockback\n{speed} Speed";
}
