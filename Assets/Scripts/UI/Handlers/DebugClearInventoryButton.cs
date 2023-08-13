using UnityEngine;

public class DebugClearInventoryButton : MonoBehaviour
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;
    [SerializeField] private Inventory inventory;

    public void DebugClearInventoryButton_OnClick()
    {
        inventory.ClearInventory();

        if (InventoryUIHeldItemController.Instance.HoldingItem())
        {
            InventoryUIHeldItemController.Instance.HideHeldItemUI();
        }
    }
}
