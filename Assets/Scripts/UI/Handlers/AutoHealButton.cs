using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoHealButton : MonoBehaviour
{
    [SerializeField] private Button autoHealButton;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private HealthController playerHealthController;

    private void Awake()
    {
        playerHealthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

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

    private void HealthController_OnHealthChanged(int _, int _1)
    {
        autoHealButton.interactable = !playerHealthController.AtMaxHealth();
    }
}
