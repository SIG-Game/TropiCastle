using UnityEngine;

public class InventoryUISwapInputHandler : MonoBehaviour
{
    [Inject] private HoveredItemSlotManager hoveredItemSlotManager;
    [Inject] private InputManager inputManager;
    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject("PlayerInventory")] private Inventory playerInventory;

    private void Awake()
    {
        this.InjectDependencies();
    }

    // Must run before ItemSelectionController Update method so that using number keys to
    // swap items in inventory UI takes priority over using number keys to select an item
    private void Update()
    {
        bool canSwapItems = inventoryUIManager.InventoryUIOpen &&
            hoveredItemSlotManager.HoveredItemIndex != -1 &&
            !inventoryUIHeldItemController.HoldingItem();
        if (!canSwapItems)
        {
            return;
        }

        int? numberKeyIndex = inputManager.GetNumberKeyIndexIfUnusedThisFrame();
        if (!numberKeyIndex.HasValue)
        {
            return;
        }

        if (playerInventory == hoveredItemSlotManager.HoveredInventory)
        {
            playerInventory.SwapItemsAt(
                hoveredItemSlotManager.HoveredItemIndex, numberKeyIndex.Value);
        }
        else
        {
            playerInventory.SwapItemsBetweenInventories(numberKeyIndex.Value,
                hoveredItemSlotManager.HoveredInventory,
                hoveredItemSlotManager.HoveredItemIndex);
        }
    }
}
