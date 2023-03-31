using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private ItemSlotContainerController itemSlotContainer;
    [SerializeField] private ItemSelectionController itemSelectionController;

    private InputAction inventoryAction;

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
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex -= ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -= ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void Inventory_ChangedItemAtIndex(ItemWithAmount item, int index)
    {
        Sprite changedItemSprite = item.itemData.sprite;

        SetInventorySpriteAtSlotIndex(changedItemSprite, index);
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        itemSlotContainer.HighlightSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        itemSlotContainer.UnhighlightSlotAtIndex(index);
    }

    public void SetInventorySpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        itemSlotContainer.SetSpriteAtSlotIndex(sprite, slotIndex);
    }

    public Inventory GetInventory() => inventory;
}
