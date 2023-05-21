using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : ItemSlotController, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int slotItemIndex;

    private Tooltip tooltipTextWithPriority;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryUIController != null)
        {
            inventoryUIController.HoveredItemIndex = slotItemIndex;
        }

        SetSlotTooltipText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryUIHeldItemController.Instance.LeftClickedItemAtIndex(
                inventory, slotItemIndex, this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryUIHeldItemController.Instance.RightClickedItemAtIndex(
                inventory, slotItemIndex, this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryUIController != null)
        {
            inventoryUIController.HoveredItemIndex = -1;
        }

        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);
    }

    public void ResetSlotTooltipText()
    {
        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);
        SetSlotTooltipText();
    }

    private void SetSlotTooltipText()
    {
        if (InventoryUITooltipController.Instance.TooltipListContains(tooltipTextWithPriority))
        {
            InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);
            Debug.Log("Attempted to add tooltip to tooltip list when a tooltip for " +
                "this item slot already existed in that list");
        }

        tooltipTextWithPriority = new Tooltip(GetSlotItemTooltipText(), 0);
        InventoryUITooltipController.Instance.AddTooltipTextWithPriority(tooltipTextWithPriority);
    }

    private string GetSlotItemTooltipText()
    {
        ItemScriptableObject slotItemData = inventory.GetItemAtIndex(slotItemIndex).itemData;

        return InventoryUITooltipController.GetItemTooltipText(slotItemData);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void SetSlotItemIndex(int slotItemIndex)
    {
        this.slotItemIndex = slotItemIndex;
    }
}
