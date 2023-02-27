using UnityEngine;

public class HotbarUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private ItemSlotContainerController itemSlotContainer;
    [SerializeField] private InventoryUIController inventoryUIController;

    private int hotbarSize;
    private int hotbarHighlightedItemSlotIndex;

    private void Awake()
    {
        inventory.ChangedItemAtIndex += Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex += ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex += ItemSelectionController_OnItemDeselectedAtIndex;
        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void Start()
    {
        hotbarSize = itemSlotContainer.GetItemSlotCount();
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

        itemSlotContainer.SetSpriteAtSlotIndex(changedItemSprite, index);
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

        itemSlotContainer.UnhighlightSlotAtIndex(index);
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

            itemSlotContainer.SetSpriteAtSlotIndex(itemSpriteAtCurrentIndex, i);
        }
    }

    private void UpdateHotbarHighlightedItemSlot()
    {
        itemSlotContainer.UnhighlightSlotAtIndex(hotbarHighlightedItemSlotIndex);
        HighlightHotbarItemSlotAtIndex(itemSelectionController.SelectedItemIndex);
    }

    private void HighlightHotbarItemSlotAtIndex(int index)
    {
        hotbarHighlightedItemSlotIndex = index;

        itemSlotContainer.HighlightSlotAtIndex(index);
    }
}
