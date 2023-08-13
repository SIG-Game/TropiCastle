using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteHeldItemButton : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;
    [SerializeField] private Inventory inventory;

    private InventoryUIHeldItemController heldItemController;

    private void Start()
    {
        heldItemController = InventoryUIHeldItemController.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (heldItemController.HoldingItem())
            {
                heldItemController.HideHeldItemUI();

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
