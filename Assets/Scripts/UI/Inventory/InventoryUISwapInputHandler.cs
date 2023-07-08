using UnityEngine;

public class InventoryUISwapInputHandler : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private InventoryUIManager inventoryUIManager;

    private InventoryUIHeldItemController heldItemController;

    private void Start()
    {
        heldItemController = InventoryUIHeldItemController.Instance;
    }

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
        bool canSwapItems = heldItemController.HoldingItem() ||
            hoveredItemSlotManager.HoveredItemIndex != -1;
        if (!canSwapItems)
        {
            return;
        }

        int numberKeyIndex = InputManager.Instance.GetNumberKeyIndexIfUnusedThisFrame();

        bool numberKeyInputAvailable = numberKeyIndex != -1;
        if (!numberKeyInputAvailable)
        {
            return;
        }

        int swapItemIndex;
        Inventory swapInventory;

        if (heldItemController.HoldingItem())
        {
            swapItemIndex = heldItemController.GetHeldItemIndex();
            swapInventory = heldItemController.GetHeldItemInventory();

            heldItemController.ResetHeldItem();
        }
        else
        {
            swapItemIndex = hoveredItemSlotManager.HoveredItemIndex;
            swapInventory = hoveredItemSlotManager.HoveredInventory;
        }

        if (inventory == swapInventory)
        {
            inventory.SwapItemsAt(swapItemIndex, numberKeyIndex);
        }
        else
        {
            inventory.SwapItemsBetweenInventories(numberKeyIndex,
                swapInventory, swapItemIndex);
        }
    }
}
