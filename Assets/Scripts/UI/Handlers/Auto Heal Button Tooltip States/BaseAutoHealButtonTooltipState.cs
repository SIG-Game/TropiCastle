public abstract class BaseAutoHealButtonTooltipState
{
    protected InventoryUITooltipController inventoryUITooltip;

    public BaseAutoHealButtonTooltipState(InventoryUITooltipController inventoryUITooltip)
    {
        this.inventoryUITooltip = inventoryUITooltip;
    }

    public abstract void StateEnter();

    public abstract void StateExit();
}
