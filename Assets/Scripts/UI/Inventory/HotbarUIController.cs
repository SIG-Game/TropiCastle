using UnityEngine;

public class HotbarUIController : ItemSlotContainerController
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private InventoryUIController inventoryUIController;

    private int hotbarSize;
    private int hotbarHighlightedItemSlotIndex;

    private void Awake()
    {
        hotbarSize = GetItemSlotCount();

        inventory.OnItemChangedAtIndex += Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex += ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex += ItemSelectionController_OnItemDeselectedAtIndex;
        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void OnDestroy()
    {
        inventory.OnItemChangedAtIndex -= Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex -= ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -= ItemSelectionController_OnItemDeselectedAtIndex;
        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    private void Inventory_ChangedItemAtIndex(ItemWithAmount item, int index)
    {
        if (index >= hotbarSize || InventoryUIController.InventoryUIOpen)
        {
            return;
        }

        UpdateSlotAtIndexUsingItem(index, item);
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        if (InventoryUIController.InventoryUIOpen)
        {
            return;
        }

        HighlightHotbarItemSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        if (InventoryUIController.InventoryUIOpen)
        {
            return;
        }

        UnhighlightSlotAtIndex(index);
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
