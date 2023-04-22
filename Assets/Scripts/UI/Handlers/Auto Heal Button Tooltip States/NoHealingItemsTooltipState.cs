public class NoHealingItemsTooltipState : BaseAutoHealButtonTooltipState
{
    private Tooltip noHealingItemsTooltip;

    public NoHealingItemsTooltipState(InventoryUITooltipController inventoryUITooltip) :
        base(inventoryUITooltip)
    {
        noHealingItemsTooltip = new Tooltip("No healing items available.", 0);
    }

    public override void StateEnter()
    {
        inventoryUITooltip.AddTooltipTextWithPriority(noHealingItemsTooltip);
    }

    public override void StateExit()
    {
        inventoryUITooltip.RemoveTooltipTextWithPriority(noHealingItemsTooltip);
    }
}

