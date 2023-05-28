using UnityEngine;

public class DeleteHeldItemButton : MonoBehaviour
{
    [SerializeField] private InventoryUIController inventoryUIController;

    private InventoryUIHeldItemController heldItemController;
    private Inventory inventory;

    private void Start()
    {
        heldItemController = InventoryUIHeldItemController.Instance;
        inventory = inventoryUIController.GetInventory();
    }

    public void DeleteHeldItemButton_OnClick()
    {
        if (heldItemController.HoldingItem())
        {
            heldItemController.HideHeldItem();

            inventory.RemoveItemAtIndex(heldItemController.GetHeldItemIndex());
        }
    }
}
