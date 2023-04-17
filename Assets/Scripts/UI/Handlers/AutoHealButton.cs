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

    private InventoryUITooltipController inventoryUITooltip;
    private Tooltip maxHealthTooltip;
    private Tooltip noHealingItemsTooltip;
    private Tooltip healingItemsToConsumeTooltip;
    private bool hoveringOverAutoHealButton;

    private void Awake()
    {
        maxHealthTooltip = new Tooltip("Already at max health.", 0);
        noHealingItemsTooltip = new Tooltip("No healing items available.", 0);

        hoveringOverAutoHealButton = false;

        playerHealthController.OnHealthChanged += HealthController_OnHealthChanged;
        playerInventory.ChangedItemAtIndex += PlayerInventory_ChangedItemAtIndex;
    }

    private void Start()
    {
        inventoryUITooltip = InventoryUITooltipController.Instance;
    }

    private void OnDestroy()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnHealthChanged -= HealthController_OnHealthChanged;
        }

        if (playerInventory != null)
        {
            playerInventory.ChangedItemAtIndex -= PlayerInventory_ChangedItemAtIndex;
        }
    }

    public void AutoHealButton_OnClick()
    {
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

        bool maxHealthTooltipShouldBeVisible =
            hoveringOverAutoHealButton && playerHealthController.AtMaxHealth();

        if (maxHealthTooltipShouldBeVisible)
        {
            if (inventoryUITooltip.TooltipListContains(healingItemsToConsumeTooltip))
            {
                inventoryUITooltip.RemoveTooltipTextWithPriority(healingItemsToConsumeTooltip);
            }
            else if (inventoryUITooltip.TooltipListContains(noHealingItemsTooltip))
            {
                inventoryUITooltip.RemoveTooltipTextWithPriority(noHealingItemsTooltip);
            }

            inventoryUITooltip.AddTooltipTextWithPriority(maxHealthTooltip);
        }
    }

    private void PlayerInventory_ChangedItemAtIndex(ItemWithAmount _, int _1)
    {
        bool shouldUpdateHealingItemsToConsumeTooltip = hoveringOverAutoHealButton &&
            inventoryUITooltip.TooltipListContains(healingItemsToConsumeTooltip);

        if (shouldUpdateHealingItemsToConsumeTooltip)
        {
            inventoryUITooltip.RemoveTooltipTextWithPriority(healingItemsToConsumeTooltip);

            UseHealingItemsTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringOverAutoHealButton = true;

        if (playerHealthController.AtMaxHealth())
        {
            inventoryUITooltip.AddTooltipTextWithPriority(maxHealthTooltip);
        }
        else
        {
            UseHealingItemsTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringOverAutoHealButton = false;

        if (playerHealthController.AtMaxHealth())
        {
            inventoryUITooltip.RemoveTooltipTextWithPriority(maxHealthTooltip);
        }
        else if (inventoryUITooltip.TooltipListContains(healingItemsToConsumeTooltip))
        {
            inventoryUITooltip.RemoveTooltipTextWithPriority(healingItemsToConsumeTooltip);
        }
        else if (inventoryUITooltip.TooltipListContains(noHealingItemsTooltip))
        {
            inventoryUITooltip.RemoveTooltipTextWithPriority(noHealingItemsTooltip);
        }
    }

    private void UseHealingItemsTooltip()
    {
        string healingItemsToConsumeNames = GetHealingItemsToConsumeNames();

        if (string.IsNullOrEmpty(healingItemsToConsumeNames))
        {
            inventoryUITooltip.AddTooltipTextWithPriority(noHealingItemsTooltip);
        }
        else
        {
            healingItemsToConsumeTooltip =
                new Tooltip($"Will Consume:\n{healingItemsToConsumeNames}", 0);

            inventoryUITooltip.AddTooltipTextWithPriority(healingItemsToConsumeTooltip);
        }
    }

    private string GetHealingItemsToConsumeNames()
    {
        StringBuilder healingItemsToConsumeStringBuilder = new StringBuilder();

        int healthBelowMax = playerHealthController.GetMaxHealth() -
            playerHealthController.GetCurrentHealth();

        int healthFromItemsToConsume = 0;

        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        for (int i = 0; i < playerInventoryItemList.Count; ++i)
        {
            if (playerInventoryItemList[i].itemData is HealingItemScriptableObject healingItem)
            {
                healingItemsToConsumeStringBuilder.AppendLine($"- {healingItem.name}");

                healthFromItemsToConsume += healingItem.healAmount;

                bool maxHealthReached = healthFromItemsToConsume >= healthBelowMax;
                if (maxHealthReached)
                {
                    break;
                }
            }
        }

        return healingItemsToConsumeStringBuilder.ToString();
    }
}
