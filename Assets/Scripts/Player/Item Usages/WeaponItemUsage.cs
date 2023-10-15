using UnityEngine;

public class WeaponItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    private WeaponItemScriptableObject strongestWeaponInInventory;

    private void Awake()
    {
        playerInventory.OnItemChangedAtIndex += PlayerInventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnItemChangedAtIndex -= PlayerInventory_OnItemChangedAtIndex;
        }
    }

    public void UseItem(ItemStack item, int _)
    {
        AttackWithWeapon((WeaponItemScriptableObject)item.itemDefinition);
    }

    public void AttackWithWeapon(WeaponItemScriptableObject weaponItemDefinition)
    {
        playerAnimator.SetFloat("Attack Speed Multiplier", weaponItemDefinition.attackSpeed);

        weaponSpriteRenderer.sprite = weaponItemDefinition.weaponSprite;
        weaponController.SetUpUsingScriptableObject(weaponItemDefinition);

        playerController.IsAttacking = true;

        playerController.DisableItemSelection();

        string attackTypeString = weaponItemDefinition.attackType.ToString();

        playerAnimator.Play($"{attackTypeString} {playerController.Direction}");
    }

    public void AttackWithStrongestWeaponInInventory()
    {
        AttackWithWeapon(strongestWeaponInInventory);
    }

    private void UpdateStrongestWeaponInInventory()
    {
        strongestWeaponInInventory = null;

        foreach (ItemStack item in playerInventory.GetItemList())
        {
            if (item.itemDefinition is not WeaponItemScriptableObject)
            {
                continue;
            }

            WeaponItemScriptableObject weapon = item.itemDefinition as WeaponItemScriptableObject;

            if (strongestWeaponInInventory == null ||
                weapon.damage > strongestWeaponInInventory.damage)
            {
                strongestWeaponInInventory = weapon;
            }
        }
    }

    private void PlayerInventory_OnItemChangedAtIndex(ItemStack _, int _1)
    {
        UpdateStrongestWeaponInInventory();
    }

    public bool PlayerHasWeapon() => strongestWeaponInInventory != null;
}
