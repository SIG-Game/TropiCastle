using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : ItemSlotContainerController
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<GameObject> inventoryUIGameObjects;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private InputActionReference inventoryActionReference;

    private InputAction inventoryAction;

    public int HoveredItemIndex { get; set; }

    public event Action OnInventoryClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIController()
    {
        InventoryUIOpen = false;
    }

    private void Awake()
    {
        inventoryAction = inventoryActionReference.action;

        HoveredItemIndex = -1;

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
        itemSelectionController.OnItemSelectedAtIndex += ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex += ItemSelectionController_OnItemDeselectedAtIndex;
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
                HoveredItemIndex = -1;

                OnInventoryClosed();
            }
        }
    }

    private void OnDestroy()
    {
        inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
        itemSelectionController.OnItemSelectedAtIndex -= ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -= ItemSelectionController_OnItemDeselectedAtIndex;
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

    private void Inventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        UpdateSlotAtIndexUsingItem(index, item);

        if (InventoryUIOpen && index == HoveredItemIndex)
        {
            (itemSlotControllers[index] as InventoryUIItemSlotController).ResetSlotTooltipText();
        }
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        HighlightSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightSlotAtIndex(index);
    }

    public void InvokeOnInventoryUIClosedEvent()
    {
        OnInventoryClosed();
    }

    public Inventory GetInventory() => inventory;
}
