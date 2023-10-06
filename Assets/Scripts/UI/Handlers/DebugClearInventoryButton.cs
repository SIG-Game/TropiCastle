using UnityEngine;

public class DebugClearInventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory inventory;

    public void DebugClearInventoryButton_OnClick()
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            inventoryUIHeldItemController.HideHeldItemUI();
        }

        inventory.ClearInventory();
    }
}
