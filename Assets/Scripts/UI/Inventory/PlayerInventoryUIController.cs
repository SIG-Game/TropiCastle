using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryUIController : ItemSlotContainerController
{
    [SerializeField] private List<GameObject> inventoryUIGameObjects;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InputActionReference inventoryActionReference;

    private InputAction inventoryAction;

    protected override void Awake()
    {
        base.Awake();

        inventoryAction = inventoryActionReference.action;
    }

    // Must run after any script Update methods that can set ActionDisablingUIOpen to true to
    // prevent an action disabling UI from opening on the same frame that the inventory UI is opened
    private void Update()
    {
        if (PlayerController.ActionDisablingUIOpen)
        {
            return;
        }

        bool closeInventoryUI = Input.GetKeyDown(KeyCode.Escape) &&
            InventoryUIManager.InventoryUIOpen;
        if (closeInventoryUI)
        {
            InputManager.Instance.EscapeKeyUsedThisFrame = true;
        }

        bool toggleInventoryUI = (inventoryAction.WasPressedThisFrame() &&
            (!PauseController.Instance.GamePaused || InventoryUIManager.InventoryUIOpen)) ||
            closeInventoryUI;
        if (toggleInventoryUI)
        {
            InventoryUIManager.InventoryUIOpen = !InventoryUIManager.InventoryUIOpen;
            PauseController.Instance.GamePaused = InventoryUIManager.InventoryUIOpen;

            inventoryUIGameObjects.ForEach(
                x => x.SetActive(InventoryUIManager.InventoryUIOpen));

            if (!InventoryUIManager.InventoryUIOpen)
            {
                inventoryUIManager.InvokeOnInventoryUIClosedEvent();
            }
        }
    }

    public Inventory GetInventory() => inventory;

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
