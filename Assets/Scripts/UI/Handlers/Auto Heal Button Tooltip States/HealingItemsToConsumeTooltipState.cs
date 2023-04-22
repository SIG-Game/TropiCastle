using UnityEngine;

public class HealingItemsToConsumeTooltipState : BaseAutoHealButtonTooltipState
{
    private Tooltip healingItemsToConsumeTooltip;
    private string tooltipTextToUseOnStateEntry;

    public HealingItemsToConsumeTooltipState(InventoryUITooltipController inventoryUITooltip) :
        base(inventoryUITooltip)
    {
    }

    public override void StateEnter()
    {
        if (tooltipTextToUseOnStateEntry == null)
        {
            Debug.LogError($"{nameof(tooltipTextToUseOnStateEntry)} is null in " +
                $"{nameof(StateEnter)} method in " +
                $"{nameof(HealingItemsToConsumeTooltipState)} class.");
            return;
        }

        healingItemsToConsumeTooltip = new Tooltip(tooltipTextToUseOnStateEntry, 0);

        inventoryUITooltip.AddTooltipTextWithPriority(healingItemsToConsumeTooltip);

        tooltipTextToUseOnStateEntry = null;
    }

    public override void StateExit()
    {
        inventoryUITooltip.RemoveTooltipTextWithPriority(healingItemsToConsumeTooltip);
    }

    public void SetTooltipTextToUseOnStateEntry(string tooltipTextToUseOnStateEntry)
    {
        this.tooltipTextToUseOnStateEntry = tooltipTextToUseOnStateEntry;
    }
}
