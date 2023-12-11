using UnityEngine;

public class DebugFillInventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory inventory;

    public void DebugFillInventoryButton_OnClick()
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            inventoryUIHeldItemController.HideHeldItemUI();
        }

        inventory.FillInventory();
    }
}
