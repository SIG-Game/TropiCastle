using UnityEngine;

public class WeaponItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    public void UseItem(ItemStack item, int itemIndex)
    {
        WeaponItemScriptableObject weaponItemDefinition =
            (WeaponItemScriptableObject)item.itemDefinition;

        playerAnimator.SetFloat("Attack Speed Multiplier",
            weaponItemDefinition.GetFloatProperty("AttackSpeed"));

        weaponSpriteRenderer.sprite = weaponItemDefinition.WeaponSprite;
        weaponController.SetUpUsingScriptableObject(weaponItemDefinition);

        playerController.IsAttacking = true;

        playerController.DisableItemSelection();

        string attackType = weaponItemDefinition.GetStringProperty("AttackType");

        playerAnimator.Play($"{attackType} {playerController.Direction}");

        if (item.instanceProperties != null &&
            item.instanceProperties.HasProperty("Durability"))
        {
            playerInventory.DecrementItemDurabilityAtIndex(itemIndex);
        }
    }
}
