using UnityEngine;

public class DebugClearInventoryButton : MonoBehaviour
{
    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject("PlayerInventory")] private Inventory playerInventory;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void DebugClearInventoryButton_OnClick()
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            inventoryUIHeldItemController.HideHeldItemUI();
        }

        playerInventory.ClearInventory();
    }
}
