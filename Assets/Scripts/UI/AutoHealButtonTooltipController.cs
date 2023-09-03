using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AutoHealButtonTooltipController : MonoBehaviour, IElementWithTooltip
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private HealthController playerHealthController;

    private const string atMaxHealthTooltipText = "Already at max health.";
    private const string noHealingItemsAvailableTooltipText = "No healing items available.";

    private string GetHealingItemsToConsumeNames()
    {
        StringBuilder healingItemsToConsumeStringBuilder = new StringBuilder();

        int healthBelowMax = playerHealthController.GetMaxHealth() -
            playerHealthController.CurrentHealth;

        int healthFromItemsToConsume = 0;

        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        for (int i = 0; i < playerInventoryItemList.Count; ++i)
        {
            if (playerInventoryItemList[i].itemDefinition is HealingItemScriptableObject healingItem)
            {
                int amountToUse = 0;
                bool maxHealthReached = false;

                for (int j = 0; j < playerInventoryItemList[i].amount; ++j)
                {
                    ++amountToUse;

                    healthFromItemsToConsume += healingItem.healAmount;

                    maxHealthReached = healthFromItemsToConsume >= healthBelowMax;
                    if (maxHealthReached)
                    {
                        break;
                    }
                }

                healingItemsToConsumeStringBuilder.AppendLine($"- {amountToUse} {healingItem.name}");

                if (maxHealthReached)
                {
                    break;
                }
            }
        }

        return healingItemsToConsumeStringBuilder.ToString();
    }

    public string GetTooltipText()
    {
        if (playerHealthController.AtMaxHealth())
        {
            return atMaxHealthTooltipText;
        }
        else
        {
            string healingItemsToConsumeNames = GetHealingItemsToConsumeNames();

            if (string.IsNullOrEmpty(healingItemsToConsumeNames))
            {
                return noHealingItemsAvailableTooltipText;
            }
            else
            {
                string healingItemsToConsumeTooltipText =
                    $"Will Consume:\n{healingItemsToConsumeNames}";

                return healingItemsToConsumeTooltipText;
            }
        }
    }
}
