using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : ItemSlotController, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventoryUIItemSlotContainerController inventoryUIItemSlotContainer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int slotItemIndex;

    private KeyValuePair<string, int> tooltipTextWithPriority;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryUIItemSlotContainer.HoveredItemIndex = slotItemIndex;
        SetSlotTooltipText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryUIHeldItemController.Instance.LeftClickedItemAtIndex(slotItemIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUIItemSlotContainer.HoveredItemIndex = -1;
        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);
    }

    public void ResetSlotTooltipText()
    {
        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);
        SetSlotTooltipText();
    }

    private void SetSlotTooltipText()
    {
        tooltipTextWithPriority = new KeyValuePair<string, int>(GetSlotItemTooltipText(), 0);
        InventoryUITooltipController.Instance.AddTooltipTextWithPriority(tooltipTextWithPriority);
    }

    private string GetSlotItemTooltipText()
    {
        ItemScriptableObject slotItemData = inventory.GetItemAtIndex(slotItemIndex).itemData;

        return InventoryUITooltipController.GetItemTooltipText(slotItemData);
    }

    public void SetSlotItemIndex(int slotItemIndex)
    {
        this.slotItemIndex = slotItemIndex;
    }
}
