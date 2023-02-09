using UnityEngine;
using UnityEngine.UI;

public class HotbarUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private Transform hotbarItemSlotContainer;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Color highlightedSlotColor;

    private Color unhighlightedSlotColor;
    private int hotbarSize;

    private void Awake()
    {
        unhighlightedSlotColor = hotbarItemSlotContainer.GetChild(0).GetComponent<Image>().color;
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
        if (index >= hotbarSize || inventoryUIController.IsInventoryUIOpen())
        {
            return;
        }

        Sprite changedItemSprite = item.itemData.sprite;

        ItemSlotContainerHelper.SetItemSlotSpriteAtIndex(hotbarItemSlotContainer, index, changedItemSprite);
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        HighlightItemSlotAtIndex(hotbarItemSlotContainer, index);
        HighlightItemSlotAtIndex(inventoryItemSlotContainer, index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightItemSlotAtIndex(hotbarItemSlotContainer, index);
        UnhighlightItemSlotAtIndex(inventoryItemSlotContainer, index);
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        UpdateSpritesInAllHotbarSlots();
    }

    private void UpdateSpritesInAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            Sprite itemSpriteAtCurrentIndex = inventory.GetItemAtIndex(i).itemData.sprite;

            ItemSlotContainerHelper.SetItemSlotSpriteAtIndex(hotbarItemSlotContainer, i, itemSpriteAtCurrentIndex);
        }
    }

    private void UnhighlightItemSlotAtIndex(Transform itemSlotContainer, int index)
    {
        ItemSlotContainerHelper.SetItemSlotColorAtIndex(itemSlotContainer, index, unhighlightedSlotColor);
    }

    private void HighlightItemSlotAtIndex(Transform itemSlotContainer, int index)
    {
        ItemSlotContainerHelper.SetItemSlotColorAtIndex(itemSlotContainer, index, highlightedSlotColor);
    }
}
