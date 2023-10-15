using System.Collections.Generic;
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

    public void UseHealingItemsUntilMaxHealthReached()
    {
        List<ItemStack> playerInventoryItemList = playerInventory.GetItemList();

        for (int i = 0; i < playerInventoryItemList.Count; ++i)
        {
            if (playerInventoryItemList[i].itemDefinition is HealingItemScriptableObject)
            {
                UseHealingItemStackAtIndex(i, out bool maxHealthReached);

                if (maxHealthReached)
                {
                    return;
                }
            }
        }
    }

    private void UseHealingItemStackAtIndex(int healingItemStackIndex,
        out bool maxHealthReached)
    {
        ItemStack healingItem =
            playerInventory.GetItemAtIndex(healingItemStackIndex);

        int initialHealingItemAmount = healingItem.amount;

        int itemHealAmount =
            (healingItem.itemDefinition as HealingItemScriptableObject).healAmount;

        for (int i = 0; i < initialHealingItemAmount; ++i)
        {
            playerHealthController.IncreaseHealth(itemHealAmount);
            playerInventory.DecrementItemStackAtIndex(healingItemStackIndex);

            if (playerHealthController.AtMaxHealth())
            {
                maxHealthReached = true;
                return;
            }
        }

        maxHealthReached = false;
    }
}
