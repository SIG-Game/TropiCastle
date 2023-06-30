using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Better integrate this class with InventoryUIController
public class ChestUIController : MonoBehaviour
{
    [SerializeField] private GameObject chestUI;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private List<InventoryUIItemSlotController> chestItemSlots;
    [SerializeField] private List<InventoryUIItemSlotController> playerItemSlots;
    [SerializeField] private InputActionReference inventoryActionReference;

    private Inventory chestInventory;
    private Inventory playerInventory;
    private InputAction inventoryAction;

    public static ChestUIController Instance;
    public static bool ChestUIOpen;

    static ChestUIController()
    {
        ChestUIOpen = false;
    }

    private void Awake()
    {
        Instance = this;

        inventoryAction = inventoryActionReference.action;
    }

    // Must run after the InventoryUIController Update method so that the inventory
    // input press for closing the chest UI is not reused to open the inventory UI
    private void Update()
    {
        bool closeChestInputPressed = inventoryAction.WasPressedThisFrame() ||
            Input.GetKeyDown(KeyCode.Escape) ||
            InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame();

        bool closeChestUI = closeChestInputPressed
            && chestUI.activeInHierarchy;
        if (closeChestUI)
        {
            ChestUIOpen = false;
            PauseController.Instance.GamePaused = false;

            chestUI.SetActive(false);

            inventoryUIController.InvokeOnInventoryUIClosedEvent();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                InputManager.Instance.EscapeKeyUsedThisFrame = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (chestInventory != null)
        {
            chestInventory.OnItemChangedAtIndex -= ChestInventory_OnItemChangedAtIndex;
        }

        if (playerInventory != null)
        {
            playerInventory.OnItemChangedAtIndex -= PlayerInventory_OnItemChangedAtIndex;
        }

        Instance = null;
    }

    public void ShowChestUI()
    {
        ChestUIOpen = true;
        PauseController.Instance.GamePaused = true;

        chestUI.SetActive(true);
    }

    public void SetChestInventory(Inventory chestInventory)
    {
        this.chestInventory = chestInventory;

        List<ItemWithAmount> chestItemList = chestInventory.GetItemList();

        for (int i = 0; i < chestItemSlots.Count; ++i)
        {
            var chestItemSlot = chestItemSlots[i];
            var chestItem = chestItemList[i];

            chestItemSlot.SetInventory(chestInventory);
            chestItemSlot.UpdateUsingItem(chestItem);
        }

        chestInventory.OnItemChangedAtIndex += ChestInventory_OnItemChangedAtIndex;
    }

    public void SetPlayerInventory(Inventory playerInventory)
    {
        this.playerInventory = playerInventory;

        SetInventorySlotSprites(playerInventory, playerItemSlots);

        playerInventory.OnItemChangedAtIndex += PlayerInventory_OnItemChangedAtIndex;
    }

    private void SetInventorySlotSprites(Inventory inventory,
        List<InventoryUIItemSlotController> itemSlots)
    {
        List<ItemWithAmount> itemList = inventory.GetItemList();

        for (int i = 0; i < itemList.Count; ++i)
        {
            itemSlots[i].UpdateUsingItem(itemList[i]);
        }
    }

    private void ChestInventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        UpdateItemSlotAtIndex(chestItemSlots, index, item);

        if (ChestUIOpen && chestInventory == hoveredItemSlotManager.HoveredInventory &&
            index == hoveredItemSlotManager.HoveredItemIndex)
        {
            chestItemSlots[index].ResetSlotTooltipText();
        }
    }

    private void PlayerInventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        UpdateItemSlotAtIndex(playerItemSlots, index, item);

        if (ChestUIOpen && playerInventory == hoveredItemSlotManager.HoveredInventory &&
            index == hoveredItemSlotManager.HoveredItemIndex)
        {
            playerItemSlots[index].ResetSlotTooltipText();
        }
    }

    private void UpdateItemSlotAtIndex(List<InventoryUIItemSlotController> itemSlots,
        int index, ItemWithAmount item)
    {
        itemSlots[index].UpdateUsingItem(item);
    }
}
