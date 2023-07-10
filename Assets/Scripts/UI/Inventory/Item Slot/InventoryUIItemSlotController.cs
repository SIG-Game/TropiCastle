using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : ItemSlotController, IElementWithTooltip,
    IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int slotItemIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoveredItemSlotManager != null)
        {
            hoveredItemSlotManager.HoveredItemIndex = slotItemIndex;
            hoveredItemSlotManager.HoveredInventory = inventory;
        }

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
        if (hoveredItemSlotManager != null)
        {
            hoveredItemSlotManager.HoveredItemIndex = -1;
        }
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

    public string GetTooltipText() => GetSlotItemTooltipText();
}
