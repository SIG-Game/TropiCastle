using UnityEngine;

public class InventoryUISwapInputHandler : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private InputManager inputManager;

    // Must run before ItemSelectionController Update method so that using number keys to
    // swap items in inventory UI takes priority over using number keys to select an item
    private void Update()
    {
        if (inventoryUIManager.InventoryUIOpen)
        {
            SwapItemsUsingNumberKeyInput();
        }
    }

    private void SwapItemsUsingNumberKeyInput()
    {
        bool canSwapItems = inventoryUIHeldItemController.HoldingItem() ||
            hoveredItemSlotManager.HoveredItemIndex != -1;
        if (!canSwapItems)
        {
            return;
        }

        int? numberKeyIndex = inputManager.GetNumberKeyIndexIfUnusedThisFrame();
        if (!numberKeyIndex.HasValue)
        {
            return;
        }

        int swapItemIndex;
        Inventory swapInventory;

        if (inventoryUIHeldItemController.HoldingItem())
        {
            swapItemIndex = inventoryUIHeldItemController.GetHeldItemIndex();
            swapInventory = inventoryUIHeldItemController.GetHeldItemInventory();

            inventoryUIHeldItemController.ResetHeldItem();
        }
        else
        {
            swapItemIndex = hoveredItemSlotManager.HoveredItemIndex;
            swapInventory = hoveredItemSlotManager.HoveredInventory;
        }

        if (inventory == swapInventory)
        {
            inventory.SwapItemsAt(swapItemIndex, numberKeyIndex.Value);
        }
        else
        {
            inventory.SwapItemsBetweenInventories(numberKeyIndex.Value,
                swapInventory, swapItemIndex);
        }
    }
}
