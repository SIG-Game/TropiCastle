using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteHeldItemButton : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory inventory;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (inventoryUIHeldItemController.HoldingItem())
            {
                inventoryUIHeldItemController.HideHeldItemUI();

                inventory.RemoveItemAtIndex(
                    inventoryUIHeldItemController.GetHeldItemIndex());
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (inventoryUIHeldItemController.HoldingItem())
            {
                inventoryUIHeldItemController.DecrementHeldItemStack();
            }
        }
    }

    public void OnPointerEnter(PointerEventData _)
    {
        inventoryUIHeldItemController.SetRightClickToResetEnabled(false);
    }

    public void OnPointerExit(PointerEventData _)
    {
        inventoryUIHeldItemController.SetRightClickToResetEnabled(true);
    }
}
