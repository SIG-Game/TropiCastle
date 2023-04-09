using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : ItemSlotContainerController
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject autoHealButton;
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
            autoHealButton.SetActive(InventoryUIOpen);

            if (!InventoryUIOpen)
            {
                OnInventoryClosed();
            }
        }

        if (InventoryUIOpen)
        {
            SwapItemsUsingNumberKeyInput();
        }
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex -= ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -= ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void SwapItemsUsingNumberKeyInput()
    {
        bool canSwapItems = InventoryUIHeldItemController.Instance.HoldingItem() ||
            HoveredItemIndex != -1;
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

        if (InventoryUIHeldItemController.Instance.HoldingItem())
        {
            swapItemIndex = InventoryUIHeldItemController.Instance.GetHeldItemIndex();

            InventoryUIHeldItemController.Instance.HideHeldItem();
        }
        else
        {
            swapItemIndex = HoveredItemIndex;
        }

        inventory.SwapItemsAt(swapItemIndex, numberKeyIndex);

        bool hoveredItemPotentiallyChanged = HoveredItemIndex == swapItemIndex ||
            HoveredItemIndex == numberKeyIndex;
        if (hoveredItemPotentiallyChanged)
        {
            UpdateInventoryTooltipAtIndex(HoveredItemIndex);
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

        SetSpriteAtSlotIndex(changedItemSprite, index);
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        HighlightSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightSlotAtIndex(index);
    }

    public Inventory GetInventory() => inventory;
}
