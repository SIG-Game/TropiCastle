using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteHeldItemButton : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inventoryUIHeldItemController.HideHeldItemUI();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventoryUIHeldItemController.DecrementHeldItemStack();
            }
        }
    }

    public void OnPointerEnter(PointerEventData _) =>
        inventoryUIHeldItemController.RightClickToResetEnabled = false;

    public void OnPointerExit(PointerEventData _) =>
        inventoryUIHeldItemController.RightClickToResetEnabled = true;
}
