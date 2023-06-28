using UnityEngine;

public class InventoryUISwapInputHandler : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;

    private InventoryUIHeldItemController heldItemController;

    private void Start()
    {
        heldItemController = InventoryUIHeldItemController.Instance;
    }

    // Must run before ItemSelectionController Update method so that using number keys to
    // swap items in inventory UI takes priority over using number keys to select an item
    private void Update()
    {
        if (InventoryUIController.InventoryUIOpen)
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

        if (heldItemController.HoldingItem())
        {
            swapItemIndex = heldItemController.GetHeldItemIndex();

            heldItemController.HideHeldItem();
        }
        else
        {
            swapItemIndex = hoveredItemSlotManager.HoveredItemIndex;
        }

        inventory.SwapItemsAt(swapItemIndex, numberKeyIndex);
    }
}
