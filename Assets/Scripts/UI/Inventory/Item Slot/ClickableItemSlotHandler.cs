using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableItemSlotHandler : MonoBehaviour, IElementWithTooltip,
    IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory inventory;
    [SerializeField] private int slotItemIndex;
    [SerializeField] private bool itemPlacementEnabled;

    public Inventory Inventory
    {
        private get => inventory;
        set => inventory = value;
    }

    public int SlotItemIndex
    {
        private get => slotItemIndex;
        set => slotItemIndex = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoveredItemSlotManager != null)
        {
            hoveredItemSlotManager.HoveredItemIndex = SlotItemIndex;
            hoveredItemSlotManager.HoveredInventory = Inventory;
        }

        if (Input.GetMouseButton(0))
        {
            inventoryUIHeldItemController.HeldLeftClickOverItemAtIndex(
                Inventory, SlotItemIndex);
        }
        else if (Input.GetMouseButton(1) && itemPlacementEnabled)
        {
            inventoryUIHeldItemController.HeldRightClickOverItemAtIndex(
                Inventory, SlotItemIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryUIHeldItemController.LeftClickedItemAtIndex(
                Inventory, SlotItemIndex, itemPlacementEnabled);
        }
        else if (eventData.button == PointerEventData.InputButton.Right &&
            itemPlacementEnabled)
        {
            inventoryUIHeldItemController.RightClickedItemAtIndex(
                Inventory, SlotItemIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoveredItemSlotManager != null)
        {
            hoveredItemSlotManager.HoveredItemIndex = -1;
        }
    }

    public string GetTooltipText() =>
        Inventory.GetItemAtIndex(SlotItemIndex).GetTooltipText();
}
