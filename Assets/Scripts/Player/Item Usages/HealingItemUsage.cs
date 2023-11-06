using UnityEngine;

public class HealingItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private Inventory playerInventory;

    public void UseItem(ItemStack item, int itemIndex)
    {
        if (!playerHealthController.AtMaxHealth())
        {
            int amountToHeal = ((HealingItemScriptableObject)item.itemDefinition).healAmount;

            playerHealthController.IncreaseHealth(amountToHeal);

            playerInventory.DecrementItemStackAtIndex(itemIndex);
        }
    }

    public void ConsumeFirstHealingItemInPlayerInventory()
    {
        int healingItemIndex = playerInventory.GetItemList().FindIndex(
            x => x.itemDefinition is HealingItemScriptableObject);

        if (healingItemIndex != -1)
        {
            ItemStack healingItem = playerInventory.GetItemAtIndex(healingItemIndex);

            UseItem(healingItem, healingItemIndex);
        }
    }
}
