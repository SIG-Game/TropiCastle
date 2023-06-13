using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : ItemSlotController, IPointerDownHandler,
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

        if (Input.GetMouseButton(0))
        {
            InventoryUIHeldItemController.Instance.HeldLeftClickOverItemAtIndex(
                inventory, slotItemIndex);
        }
        else if (Input.GetMouseButton(1))
        {
            InventoryUIHeldItemController.Instance.HeldRightClickOverItemAtIndex(
                inventory, slotItemIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryUIHeldItemController.Instance.LeftClickedItemAtIndex(
                inventory, slotItemIndex, this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryUIHeldItemController.Instance.RightClickedItemAtIndex(
                inventory, slotItemIndex);
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
        ItemWithAmount slotItem = inventory.GetItemAtIndex(slotItemIndex);

        return slotItem.GetTooltipText();
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
