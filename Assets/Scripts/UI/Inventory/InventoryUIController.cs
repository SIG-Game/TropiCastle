using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : ItemSlotContainerController
{
    [SerializeField] private List<GameObject> inventoryUIGameObjects;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private InputActionReference inventoryActionReference;

    private InputAction inventoryAction;

    public event Action OnInventoryClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIController()
    {
        InventoryUIOpen = false;
    }

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

        bool closeInventoryUI = Input.GetKeyDown(KeyCode.Escape) && InventoryUIOpen;

        if (closeInventoryUI)
        {
            InputManager.Instance.EscapeKeyUsedThisFrame = true;
        }

        bool toggleInventoryUI = (inventoryAction.WasPressedThisFrame() &&
            (!PauseController.Instance.GamePaused || InventoryUIOpen)) ||
            closeInventoryUI;
        if (toggleInventoryUI)
        {
            InventoryUIOpen = !InventoryUIOpen;
            PauseController.Instance.GamePaused = InventoryUIOpen;

            inventoryUIGameObjects.ForEach(x => x.SetActive(InventoryUIOpen));

            if (!InventoryUIOpen)
            {
                hoveredItemSlotManager.HoveredItemIndex = -1;

                OnInventoryClosed();
            }
        }
    }

    protected override void Inventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        base.Inventory_OnItemChangedAtIndex(item, index);

        if (InventoryUIOpen && index == hoveredItemSlotManager.HoveredItemIndex)
        {
            (itemSlotControllers[index] as InventoryUIItemSlotController).ResetSlotTooltipText();
        }
    }

    public void InvokeOnInventoryUIClosedEvent()
    {
        OnInventoryClosed();
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
