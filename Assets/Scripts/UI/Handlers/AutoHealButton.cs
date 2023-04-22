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

    private NoAutoHealButtonTooltipState noAutoHealButtonTooltipState;
    private AtMaxHealthTooltipState atMaxHealthTooltipState;
    private NoHealingItemsTooltipState noHealingItemsTooltipState;
    private HealingItemsToConsumeTooltipState healingItemsToConsumeTooltipState;

    private BaseAutoHealButtonTooltipState currentTooltipState;
    private InventoryUITooltipController inventoryUITooltip;
    private bool hoveringOverAutoHealButton;

    private void Awake()
    {
        hoveringOverAutoHealButton = false;

        playerHealthController.OnHealthSet += HealthController_OnHealthSet;
        playerInventory.ChangedItemAtIndex += PlayerInventory_ChangedItemAtIndex;
    }

    private void Start()
    {
        inventoryUITooltip = InventoryUITooltipController.Instance;

        noAutoHealButtonTooltipState = new NoAutoHealButtonTooltipState(inventoryUITooltip);
        atMaxHealthTooltipState = new AtMaxHealthTooltipState(inventoryUITooltip);
        noHealingItemsTooltipState = new NoHealingItemsTooltipState(inventoryUITooltip);
        healingItemsToConsumeTooltipState = new HealingItemsToConsumeTooltipState(inventoryUITooltip);

        currentTooltipState = noAutoHealButtonTooltipState;

        currentTooltipState.StateEnter();
    }

    private void OnDestroy()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnHealthSet -= HealthController_OnHealthSet;
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

    private void HealthController_OnHealthSet(int _)
    {
        autoHealButton.interactable = !playerHealthController.AtMaxHealth();

        bool maxHealthTooltipShouldBeVisible =
            hoveringOverAutoHealButton && playerHealthController.AtMaxHealth();

        if (maxHealthTooltipShouldBeVisible)
        {
            SwitchState(atMaxHealthTooltipState);
        }
    }

    private void PlayerInventory_ChangedItemAtIndex(ItemWithAmount _, int _1)
    {
        bool shouldUpdateHealingItemsTooltip = hoveringOverAutoHealButton &&
            (currentTooltipState == noHealingItemsTooltipState ||
            currentTooltipState == healingItemsToConsumeTooltipState);

        if (shouldUpdateHealingItemsTooltip)
        {
            SwitchToHealingItemsTooltipState();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringOverAutoHealButton = true;

        if (playerHealthController.AtMaxHealth())
        {
            SwitchState(atMaxHealthTooltipState);
        }
        else
        {
            SwitchToHealingItemsTooltipState();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringOverAutoHealButton = false;

        SwitchState(noAutoHealButtonTooltipState);
    }

    private void SwitchToHealingItemsTooltipState()
    {
        string healingItemsToConsumeNames = GetHealingItemsToConsumeNames();

        if (string.IsNullOrEmpty(healingItemsToConsumeNames))
        {
            SwitchState(noHealingItemsTooltipState);
        }
        else
        {
            string healingItemsToConsumeTooltipText =
                $"Will Consume:\n{healingItemsToConsumeNames}";

            healingItemsToConsumeTooltipState
                .SetTooltipTextToUseOnStateEntry(healingItemsToConsumeTooltipText);

            SwitchState(healingItemsToConsumeTooltipState);
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

    public void SwitchState(BaseAutoHealButtonTooltipState newState)
    {
        currentTooltipState.StateExit();
        currentTooltipState = newState;
        currentTooltipState.StateEnter();
    }
}
