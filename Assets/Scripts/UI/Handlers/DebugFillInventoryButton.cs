using UnityEngine;

public class DebugFillInventoryButton : MonoBehaviour
{
    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject("PlayerInventory")] private Inventory playerInventory;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void DebugFillInventoryButton_OnClick()
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            inventoryUIHeldItemController.HideHeldItemUI();
        }

        playerInventory.FillInventory();
    }
}
