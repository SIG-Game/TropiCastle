using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerInventoryUIController : InventoryUIWithSelectionController
{
    [SerializeField] private List<GameObject> playerInventoryUIGameObjects;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private InputManager inputManager;

    // Must run after any script Update methods that can set ActionDisablingUIOpen
    // to true to prevent an action disabling UI from opening on the same frame
    // that the player inventory UI is opened
    private void Update()
    {
        if (playerActionDisablingUIManager.ActionDisablingUIOpen)
        {
            return;
        }

        bool openPlayerInventoryUI =
            !PauseController.Instance.GamePaused &&
            inputManager.GetInventoryButtonDownIfUnusedThisFrame();
        if (openPlayerInventoryUI)
        {
            inventoryUIManager.SetCurrentInventoryUIGameObjects(playerInventoryUIGameObjects);
            inventoryUIManager.SetCanCloseUsingInteractAction(false);
            inventoryUIManager.EnableCurrentInventoryUI();
        }
    }

    [ContextMenu("Set Inventory UI Item Slot Indexes")]
    private void SetInventoryUIItemSlotIndexes()
    {
        InventoryUIItemSlotController[] childInventoryUIItemSlots =
            GetComponentsInChildren<InventoryUIItemSlotController>(true);

        Undo.RecordObjects(childInventoryUIItemSlots, "Set Inventory UI Item Slot Indexes");

        int currentSlotItemIndex = 0;
        foreach (InventoryUIItemSlotController inventoryUIItemSlot in
            childInventoryUIItemSlots)
        {
            inventoryUIItemSlot.SetSlotItemIndex(currentSlotItemIndex);
            ++currentSlotItemIndex;
        }
    }
}
