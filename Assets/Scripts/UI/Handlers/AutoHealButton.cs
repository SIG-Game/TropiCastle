using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoHealButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button autoHealButton;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private HealingItemUsage healingItemUsage;

    private Tooltip currentTooltip;
    private Tooltip atMaxHealthTooltip;
    private Tooltip noHealingItemsTooltip;
    private InventoryUITooltipController inventoryUITooltip;
    private bool hoveringOverAutoHealButton;

    private void Awake()
    {
        atMaxHealthTooltip = new Tooltip("Already at max health.", 0);
        noHealingItemsTooltip = new Tooltip("No healing items available.", 0);

        hoveringOverAutoHealButton = false;

        playerHealthController.OnHealthSet += HealthController_OnHealthSet;
        playerInventory.OnItemChangedAtIndex += PlayerInventory_ChangedItemAtIndex;
    }

    private void Start()
    {
        inventoryUITooltip = InventoryUITooltipController.Instance;
    }

    private void OnDestroy()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnHealthSet -= HealthController_OnHealthSet;
        }

        if (playerInventory != null)
        {
            playerInventory.OnItemChangedAtIndex -= PlayerInventory_ChangedItemAtIndex;
        }
    }

    public void AutoHealButton_OnClick()
    {
        healingItemUsage.UseHealingItemsUntilMaxHealthReached();
    }

    private void HealthController_OnHealthSet(int _)
    {
        autoHealButton.interactable = !playerHealthController.AtMaxHealth();

        bool maxHealthTooltipShouldBeVisible =
            hoveringOverAutoHealButton && playerHealthController.AtMaxHealth();

        if (maxHealthTooltipShouldBeVisible)
        {
            SetTooltip(atMaxHealthTooltip);
        }
    }

    private void PlayerInventory_ChangedItemAtIndex(ItemWithAmount _, int _1)
    {
        bool shouldUseHealingItemsTooltip = hoveringOverAutoHealButton &&
            currentTooltip != atMaxHealthTooltip;

        if (shouldUseHealingItemsTooltip)
        {
            UseHealingItemsTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringOverAutoHealButton = true;

        if (playerHealthController.AtMaxHealth())
        {
            SetTooltip(atMaxHealthTooltip);
        }
        else
        {
            UseHealingItemsTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringOverAutoHealButton = false;

        inventoryUITooltip.RemoveTooltipTextWithPriority(currentTooltip);
    }

    private void UseHealingItemsTooltip()
    {
        string healingItemsToConsumeNames = GetHealingItemsToConsumeNames();

        if (string.IsNullOrEmpty(healingItemsToConsumeNames))
        {
            SetTooltip(noHealingItemsTooltip);
        }
        else
        {
            string healingItemsToConsumeTooltipText =
                $"Will Consume:\n{healingItemsToConsumeNames}";

            Tooltip healingItemsToConsumeTooltip =
                new Tooltip(healingItemsToConsumeTooltipText, 0);

            SetTooltip(healingItemsToConsumeTooltip);
        }
    }

    private string GetHealingItemsToConsumeNames()
    {
        StringBuilder healingItemsToConsumeStringBuilder = new StringBuilder();

        int healthBelowMax = playerHealthController.GetMaxHealth() -
            playerHealthController.CurrentHealth;

        int healthFromItemsToConsume = 0;

        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        for (int i = 0; i < playerInventoryItemList.Count; ++i)
        {
            if (playerInventoryItemList[i].itemData is HealingItemScriptableObject healingItem)
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

    private void SetTooltip(Tooltip newTooltip)
    {
        if (inventoryUITooltip.TooltipListContains(currentTooltip))
        {
            inventoryUITooltip.RemoveTooltipTextWithPriority(currentTooltip);
        }

        currentTooltip = newTooltip;

        inventoryUITooltip.AddTooltipTextWithPriority(currentTooltip);
    }
}
