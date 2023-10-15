using UnityEngine;

public class HotbarUIController : InventoryUIWithSelectionController
{
    [SerializeField] private InventoryUIManager inventoryUIManager;

    private int hotbarSize;
    private int hotbarHighlightedItemSlotIndex;

    protected override void Awake()
    {
        base.Awake();

        hotbarSize = itemSlotControllers.Count;

        inventoryUIManager.OnInventoryUIClosed += InventoryUIManager_OnInventoryUIClosed;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        inventoryUIManager.OnInventoryUIClosed -= InventoryUIManager_OnInventoryUIClosed;
    }

    protected override void Inventory_OnItemChangedAtIndex(ItemStack item, int index)
    {
        if (index < hotbarSize && !inventoryUIManager.InventoryUIOpen)
        {
            base.Inventory_OnItemChangedAtIndex(item, index);
        }
    }

    protected override void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        if (!inventoryUIManager.InventoryUIOpen)
        {
            HighlightHotbarItemSlotAtIndex(index);
        }
    }

    protected override void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        if (!inventoryUIManager.InventoryUIOpen)
        {
            base.ItemSelectionController_OnItemDeselectedAtIndex(index);
        }
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        UpdateAllHotbarSlots();
        UpdateHotbarHighlightedItemSlot();
    }

    private void UpdateAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            ItemStack currentItem = inventory.GetItemAtIndex(i);

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
