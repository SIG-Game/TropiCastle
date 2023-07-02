using UnityEngine;

public class HotbarUIController : ItemSlotContainerController
{
    [SerializeField] private InventoryUIController inventoryUIController;

    private int hotbarSize;
    private int hotbarHighlightedItemSlotIndex;

    protected override void Awake()
    {
        base.Awake();

        hotbarSize = itemSlotControllers.Count;

        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    protected override void Inventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        if (index < hotbarSize && !InventoryUIController.InventoryUIOpen
            && !ChestUIController.ChestUIOpen)
        {
            base.Inventory_OnItemChangedAtIndex(item, index);
        }
    }

    protected override void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        if (!InventoryUIController.InventoryUIOpen &&
            !ChestUIController.ChestUIOpen)
        {
            HighlightHotbarItemSlotAtIndex(index);
        }
    }

    protected override void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        if (!InventoryUIController.InventoryUIOpen &&
            !ChestUIController.ChestUIOpen)
        {
            base.ItemSelectionController_OnItemDeselectedAtIndex(index);
        }
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        UpdateAllHotbarSlots();
        UpdateHotbarHighlightedItemSlot();
    }

    private void UpdateAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            ItemWithAmount currentItem = inventory.GetItemAtIndex(i);

            UpdateSlotAtIndexUsingItem(i, currentItem);
        }
    }

    private void UpdateHotbarHighlightedItemSlot()
    {
        UnhighlightSlotAtIndex(hotbarHighlightedItemSlotIndex);
        HighlightHotbarItemSlotAtIndex(itemSelectionController.SelectedItemIndex);
    }

    private void HighlightHotbarItemSlotAtIndex(int index)
    {
        hotbarHighlightedItemSlotIndex = index;

        HighlightSlotAtIndex(index);
    }
}
