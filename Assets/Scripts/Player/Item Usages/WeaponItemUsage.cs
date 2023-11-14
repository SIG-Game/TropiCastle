using UnityEngine;

public class WeaponItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    public void UseItem(ItemStack item, int _)
    {
        WeaponItemScriptableObject weaponItemDefinition =
            (WeaponItemScriptableObject)item.itemDefinition;

        playerAnimator.SetFloat("Attack Speed Multiplier",
            weaponItemDefinition.attackSpeed);

        weaponSpriteRenderer.sprite = weaponItemDefinition.weaponSprite;
        weaponController.SetUpUsingScriptableObject(weaponItemDefinition);

        playerController.IsAttacking = true;

        playerController.DisableItemSelection();

        string attackType = weaponItemDefinition.GetProperty("AttackType");

        playerAnimator.Play($"{attackType} {playerController.Direction}");
    }
}
