using UnityEngine;

public class WeaponItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    public void UseItem(ItemWithAmount item, int _)
    {
        AttackWithWeapon((WeaponItemScriptableObject)item.itemData);
    }

    public void AttackWithWeapon(WeaponItemScriptableObject weaponItemData)
    {
        playerAnimator.SetFloat("Attack Speed Multiplier", weaponItemData.attackSpeed);

        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.Damage = weaponItemData.damage;
        weaponController.EnemyKnockbackForce = weaponItemData.knockback;

        playerController.IsAttacking = true;

        playerController.DisableItemSelection();

        string attackTypeString = weaponItemData.attackType.ToString();

        playerAnimator.Play($"{attackTypeString} {playerController.Direction}");
    }
}
