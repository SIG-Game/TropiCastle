using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteHeldItemButton : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventoryUIController inventoryUIController;

    private InventoryUIHeldItemController heldItemController;
    private Inventory inventory;

    private void Start()
    {
        heldItemController = InventoryUIHeldItemController.Instance;
        inventory = inventoryUIController.GetInventory();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (heldItemController.HoldingItem())
            {
                heldItemController.HideHeldItem();

                inventory.RemoveItemAtIndex(heldItemController.GetHeldItemIndex());
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (heldItemController.HoldingItem())
            {
                heldItemController.DecrementHeldItemStack();
            }
        }
    }

    public void OnPointerEnter(PointerEventData _)
    {
        heldItemController.SetRightClickToResetEnabled(false);
    }

    public void OnPointerExit(PointerEventData _)
    {
        heldItemController.SetRightClickToResetEnabled(true);
    }
}
