using UnityEngine;

public class ChestPlayerInventoryUIController : ItemSlotContainerController
{
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;

    protected override void Inventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        base.Inventory_OnItemChangedAtIndex(item, index);

        if (ChestUIController.ChestUIOpen &&
            inventory == hoveredItemSlotManager.HoveredInventory &&
            index == hoveredItemSlotManager.HoveredItemIndex)
        {
            (itemSlotControllers[index] as InventoryUIItemSlotController).ResetSlotTooltipText();
        }
    }
}
