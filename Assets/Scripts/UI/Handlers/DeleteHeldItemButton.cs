using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteHeldItemButton : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;

    private InventoryUIHeldItemController heldItemController;
    private Inventory playerInventory;

    private void Start()
    {
        heldItemController = InventoryUIHeldItemController.Instance;
        playerInventory = playerInventoryUIController.GetInventory();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (heldItemController.HoldingItem())
            {
                heldItemController.HideHeldItem();

                playerInventory.RemoveItemAtIndex(heldItemController.GetHeldItemIndex());
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
