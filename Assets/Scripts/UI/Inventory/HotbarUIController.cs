using UnityEngine;

public class HotbarUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private Transform hotbarItemSlotContainer;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Color highlightedSlotColor;

    private Color unhighlightedSlotColor;
    private int hotbarSize;
    private int hotbarHighlightedItemSlotIndex;

    private void Awake()
    {
        unhighlightedSlotColor = ItemSlotContainerHelper.GetUnhighlightedSlotColor(hotbarItemSlotContainer);
        hotbarSize = hotbarItemSlotContainer.childCount;

        inventory.ChangedItemAtIndex += Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex += ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex += ItemSelectionController_OnItemDeselectedAtIndex;
        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
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

        Sprite changedItemSprite = item.itemData.sprite;

        ItemSlotContainerHelper.SetItemSlotSpriteAtIndex(hotbarItemSlotContainer, index, changedItemSprite);
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

        UnhighlightHotbarItemSlotAtIndex(index);
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        UpdateSpritesInAllHotbarSlots();
        UpdateHotbarHighlightedItemSlot();
    }

    private void UpdateSpritesInAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            Sprite itemSpriteAtCurrentIndex = inventory.GetItemAtIndex(i).itemData.sprite;

            ItemSlotContainerHelper.SetItemSlotSpriteAtIndex(hotbarItemSlotContainer, i, itemSpriteAtCurrentIndex);
        }
    }

    private void UpdateHotbarHighlightedItemSlot()
    {
        UnhighlightHotbarItemSlotAtIndex(hotbarHighlightedItemSlotIndex);
        HighlightHotbarItemSlotAtIndex(itemSelectionController.SelectedItemIndex);
    }

    private void HighlightHotbarItemSlotAtIndex(int index)
    {
        hotbarHighlightedItemSlotIndex = index;

        ItemSlotContainerHelper.SetItemSlotColorAtIndex(hotbarItemSlotContainer, index, highlightedSlotColor);
    }

    private void UnhighlightHotbarItemSlotAtIndex(int index)
    {
        ItemSlotContainerHelper.SetItemSlotColorAtIndex(hotbarItemSlotContainer, index, unhighlightedSlotColor);
    }
}
