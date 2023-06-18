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

    public void UseItem(ItemWithAmount item, int _)
    {
        AttackWithWeapon((WeaponItemScriptableObject)item.itemData);
    }

    public void AttackWithWeapon(WeaponItemScriptableObject weaponItemData)
    {
        playerAnimator.SetFloat("Attack Speed Multiplier", weaponItemData.attackSpeed);

        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.SetUpUsingScriptableObject(weaponItemData);

        playerController.IsAttacking = true;

        playerController.DisableItemSelection();

        string attackTypeString = weaponItemData.attackType.ToString();

        playerAnimator.Play($"{attackTypeString} {playerController.Direction}");
    }

    public void AttackWithStrongestWeaponInInventory()
    {
        AttackWithWeapon(strongestWeaponInInventory);
    }

    private void UpdateStrongestWeaponInInventory()
    {
        strongestWeaponInInventory = null;

        foreach (ItemWithAmount item in playerInventory.GetItemList())
        {
            if (item.itemData is not WeaponItemScriptableObject)
            {
                continue;
            }

            WeaponItemScriptableObject weapon = item.itemData as WeaponItemScriptableObject;

            if (strongestWeaponInInventory == null ||
                weapon.damage > strongestWeaponInInventory.damage)
            {
                strongestWeaponInInventory = weapon;
            }
        }
    }

    private void PlayerInventory_OnItemChangedAtIndex(ItemWithAmount _, int _1)
    {
        UpdateStrongestWeaponInInventory();
    }

    public bool PlayerHasWeapon() => strongestWeaponInInventory != null;
}
