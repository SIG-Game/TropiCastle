public class HotbarUIController : InventoryUIController
{
    private int hotbarSize;

    protected override void Awake()
    {
        base.Awake();

        hotbarSize = itemSlotControllers.Count;
    }

    protected override void Inventory_OnItemChangedAtIndex(ItemStack item, int index)
    {
        if (index < hotbarSize)
        {
            base.Inventory_OnItemChangedAtIndex(item, index);
        }
    }
}
