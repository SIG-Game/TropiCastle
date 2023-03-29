using System.Collections.Generic;
using UnityEngine;

public class AutoHealButton : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private HealthController playerHealthController;

    public void AutoHealButton_OnClick()
    {
        if (playerHealthController.AtMaxHealth())
        {
            return;
        }

        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        for (int i = 0; i < playerInventoryItemList.Count; ++i)
        {
            if (playerInventoryItemList[i].itemData is HealingItemScriptableObject healingItem)
            {
                playerInventory.RemoveItemAtIndex(i);
                playerHealthController.IncreaseHealth(healingItem.healAmount);

                if (playerHealthController.AtMaxHealth())
                {
                    return;
                }
            }
        }
    }
}