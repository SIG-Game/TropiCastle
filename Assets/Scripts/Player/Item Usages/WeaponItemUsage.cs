using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WeaponItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    private Dictionary<string, Sprite> weaponNameToSprite;

    private void Awake()
    {
        weaponNameToSprite = new Dictionary<string, Sprite>();

        var weaponSpritesLoadHandle = Addressables
            .LoadAssetsAsync<Sprite>("weapon sprite", null);

        var weaponSprites = weaponSpritesLoadHandle.WaitForCompletion();

        foreach (var weaponSprite in weaponSprites)
        {
            weaponNameToSprite[weaponSprite.name] = weaponSprite;
        }

        if (weaponSpritesLoadHandle.IsValid())
        {
            Addressables.Release(weaponSpritesLoadHandle);
        }
    }

    public void UseItem(ItemStack item, int itemIndex)
    {
        playerAnimator.SetFloat("Attack Speed Multiplier",
            item.ItemDefinition.GetFloatProperty("AttackSpeed"));

        weaponSpriteRenderer.sprite = weaponNameToSprite[item.ItemDefinition.name];

        weaponController.SetUpUsingScriptableObject(item.ItemDefinition);

        playerController.IsAttacking = true;

        playerController.DisableItemSelection();

        string attackType = item.ItemDefinition.GetStringProperty("AttackType");

        playerAnimator.Play($"{attackType} {playerController.Direction}");

        if (item.InstanceProperties != null &&
            item.InstanceProperties.HasProperty("Durability"))
        {
            playerInventory.DecrementItemDurabilityAtIndex(itemIndex);
        }
    }
}
