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

        if (inventory == hoveredItemSlotManager.HoveredInventory)
        {
            inventory.SwapItemsAt(
                hoveredItemSlotManager.HoveredItemIndex, numberKeyIndex.Value);
        }
        else
        {
            inventory.SwapItemsBetweenInventories(numberKeyIndex.Value,
                hoveredItemSlotManager.HoveredInventory,
                hoveredItemSlotManager.HoveredItemIndex);
        }
    }
}
