using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : ItemSlotContainerController
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private ItemSelectionController itemSelectionController;

    private InputAction inventoryAction;

    public int HoveredItemIndex { private get; set; }

    public event Action OnInventoryClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIController()
    {
        InventoryUIOpen = false;
    }

    private void Awake()
    {
        inventory.ChangedItemAtIndex += Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex += ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex += ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void Start()
    {
        inventoryAction = InputManager.Instance.GetAction("Inventory");
    }

    // Must run after any script Update methods that can set ActionDisablingUIOpen to true to
    // prevent an action disabling UI from opening on the same frame that the inventory UI is opened
    // Must run before ItemSelectionController Update method so that using number keys to swap items
    // in inventory UI takes priority over using number keys to select an item
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
            craftingUI.SetActive(InventoryUIOpen);
            inventoryUI.SetActive(InventoryUIOpen);

            if (!InventoryUIOpen)
            {
                OnInventoryClosed();
            }
        }

        if (InventoryUIOpen &&
            (InventoryUIHeldItemController.Instance.HoldingItem() || HoveredItemIndex != -1))
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
                    UpdateInventoryTooltipAtIndex(HoveredItemIndex);
                }
            }
        }
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex -= ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -= ItemSelectionController_OnItemDeselectedAtIndex;
    }

    public void UpdateInventoryTooltipAtIndex(int itemIndex)
    {
        (itemSlotControllers[itemIndex] as InventoryUIItemSlotController).ResetSlotTooltipText();
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

    private void Inventory_ChangedItemAtIndex(ItemWithAmount item, int index)
    {
        Sprite changedItemSprite = item.itemData.sprite;

        SetInventorySpriteAtSlotIndex(changedItemSprite, index);
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        HighlightSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightSlotAtIndex(index);
    }

    public void SetInventorySpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        SetSpriteAtSlotIndex(sprite, slotIndex);
    }

    public Inventory GetInventory() => inventory;
}
