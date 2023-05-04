using UnityEngine;
using UnityEngine.UI;

public class DeleteHeldItemButton : MonoBehaviour
{
    [SerializeField] private Button deleteHeldItemButton;
    [SerializeField] private InventoryUIController inventoryUIController;

    private Inventory inventory;

    private void Awake()
    {
        inventory = inventoryUIController.GetInventory();
    }

    public void DeleteHeldItemButton_OnClick()
    {
        if (!InventoryUIHeldItemController.Instance.HoldingItem())
        {
            return;
        }

        InventoryUIHeldItemController.Instance.HideHeldItem();

        inventory.RemoveItemAtIndex(InventoryUIHeldItemController.Instance.GetHeldItemIndex());
    }
}
