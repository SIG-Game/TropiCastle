using UnityEngine;

public class DebugClearInventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory inventory;

    public void DebugClearInventoryButton_OnClick()
    {
        inventory.ClearInventory();

        if (inventoryUIHeldItemController.HoldingItem())
        {
            inventoryUIHeldItemController.HideHeldItemUI();
        }
    }
}
