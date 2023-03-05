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
        inventoryUIItemSlotContainer.HoveredItemIndex = slotItemIndex;

        InventoryTooltipController.Instance.SetHoveredTooltipText(GetSlotItemTooltipText());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryUIHeldItemController.Instance.LeftClickedItemAtIndex(slotItemIndex);

            if (InventoryUIHeldItemController.Instance.HoldingItem())
            {
                InventoryTooltipController.Instance.SetHeldItemTooltipText(GetSlotItemTooltipText());
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUIItemSlotContainer.HoveredItemIndex = -1;
        InventoryTooltipController.Instance.ClearHoveredTooltipText();
    }

    private string GetSlotItemTooltipText()
    {
        ItemScriptableObject slotItemData = inventory.GetItemAtIndex(slotItemIndex).itemData;

        return InventoryTooltipController.GetItemTooltipText(slotItemData);
    }

    public void SetSlotItemIndex(int slotItemIndex)
    {
        this.slotItemIndex = slotItemIndex;
    }
}
