using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryUIItemSlotContainerController : ItemSlotContainerController
{
    [SerializeField] private InventoryUIController inventoryUIController;

    public int HoveredItemIndex { private get; set; }

    private Inventory inventory;
    private KeyValuePair<string, int> tooltipTextWithPriority;

    private void Awake()
    {
        inventory = inventoryUIController.GetInventory();
        HoveredItemIndex = -1;
    }

    // Must run before ItemSelectionController Update method so that
    // InputManager.Instance.NumberKeyUsedThisFrame can be set to true
    // before that variable is checked in ItemSelectionController
    private void Update()
    {
        if (InventoryUIHeldItemController.Instance.HoldingItem() || HoveredItemIndex != -1)
        {
            int numberKeyIndex = -1;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                numberKeyIndex = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                numberKeyIndex = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                numberKeyIndex = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                numberKeyIndex = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                numberKeyIndex = 4;
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                numberKeyIndex = 5;
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                numberKeyIndex = 6;
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                numberKeyIndex = 7;
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                numberKeyIndex = 8;
            else if (Input.GetKeyDown(KeyCode.Alpha0))
                numberKeyIndex = 9;

            if (numberKeyIndex != -1)
            {
                int swapItemIndex;

                if (InventoryUIHeldItemController.Instance.HoldingItem())
                {
                    swapItemIndex = InventoryUIHeldItemController.Instance.GetHeldItemIndex();

                    InventoryUIHeldItemController.Instance.HideHeldItem();
                }
                else
                {
                    swapItemIndex = HoveredItemIndex;
                }

                InputManager.Instance.NumberKeyUsedThisFrame = true;
                inventory.SwapItemsAt(swapItemIndex, numberKeyIndex);

                if (HoveredItemIndex != -1)
                {
                    ItemScriptableObject hoveredItem = inventory.GetItemAtIndex(HoveredItemIndex).itemData;

                    UpdateInventoryTooltipAtIndex(HoveredItemIndex);
                }
            }
        }
    }

    public void UpdateInventoryTooltipAtIndex(int itemIndex)
    {
        (itemSlotControllers[itemIndex] as InventoryUIItemSlotController).ResetSlotTooltipText();
    }

    [ContextMenu("Set Inventory UI Item Slot Indexes")]
    private void SetInventoryUIItemSlotIndexes()
    {
        InventoryUIItemSlotController[] childInventoryUIItemSlots =
            GetComponentsInChildren<InventoryUIItemSlotController>();

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
