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

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveredItemSlotManager.HoveredInventory = Inventory;
        hoveredItemSlotManager.HoveredItemIndex = slotItemIndex;

        if (Input.GetMouseButton(0))
        {
            inventoryUIHeldItemController.HeldLeftClickOverItemAtIndex(
                Inventory, slotItemIndex);
        }
        else if (Input.GetMouseButton(1))
        {
            inventoryUIHeldItemController.HeldRightClickOverItemAtIndex(
                Inventory, slotItemIndex, itemPlacementEnabled);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryUIHeldItemController.LeftClickedItemAtIndex(
                Inventory, slotItemIndex, itemPlacementEnabled);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryUIHeldItemController.RightClickedItemAtIndex(
                Inventory, slotItemIndex, itemPlacementEnabled);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveredItemSlotManager.HoveredItemIndex = -1;
    }

    public string GetTooltipText() =>
        Inventory.GetItemAtIndex(slotItemIndex).GetTooltipText();
}
