using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : ItemSlotController, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventoryUIItemSlotContainerController inventoryUIItemSlotContainer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int slotItemIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIHeldItemController.Instance.HoldingItem())
        {
            return;
        }

        inventoryUIItemSlotContainer.HoveredItemIndex = slotItemIndex;

        ItemScriptableObject slotItemData = inventory.GetItemAtIndex(slotItemIndex).itemData;

        if (slotItemData.name != "Empty")
        {
            string tooltipText = slotItemData switch
            {
                HealingItemScriptableObject healingItem =>
                    $"{slotItemData.name}\nHeals {healingItem.healAmount} Health",
                WeaponItemScriptableObject weaponItem =>
                    $"{slotItemData.name}\nDeals {weaponItem.damage} Damage",
                _ => slotItemData.name
            };

            InventoryTooltipController.Instance.ShowTooltipWithText(tooltipText);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryUIItemSlotContainer.HoveredItemIndex = -1;
            InventoryUIHeldItemController.Instance.LeftClickedItemAtIndex(slotItemIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!InventoryUIHeldItemController.Instance.HoldingItem())
        {
            inventoryUIItemSlotContainer.HoveredItemIndex = -1;
            InventoryTooltipController.Instance.ShowTooltipWithText(string.Empty);
        }
    }

    public void SetSlotItemIndex(int slotItemIndex)
    {
        this.slotItemIndex = slotItemIndex;
    }
}
