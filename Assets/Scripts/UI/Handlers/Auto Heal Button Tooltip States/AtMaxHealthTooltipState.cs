public class AtMaxHealthTooltipState : BaseAutoHealButtonTooltipState
{
    private Tooltip atMaxHealthTooltip;

    public AtMaxHealthTooltipState(InventoryUITooltipController inventoryUITooltip) :
        base(inventoryUITooltip)
    {
        atMaxHealthTooltip = new Tooltip("Already at max health.", 0);
    }

    public override void StateEnter()
    {
        inventoryUITooltip.AddTooltipTextWithPriority(atMaxHealthTooltip);
    }

    public override void StateExit()
    {
        inventoryUITooltip.RemoveTooltipTextWithPriority(atMaxHealthTooltip);
    }
}
