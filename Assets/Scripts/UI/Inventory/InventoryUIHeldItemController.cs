using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour, IElementWithTooltip
{
    [SerializeField] private GameObject heldItemUI;
    [SerializeField] private TextMeshProUGUI heldItemAmountText;
    [SerializeField] private ItemDurabilityMeterController durabilityMeter;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private Sprite transparentSprite;

    private Inventory clickedInventory;
    private Inventory heldItemInventory;
    private ItemWithAmount heldItem;
    private InventoryUIItemSlotController heldItemSlot;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private int heldItemIndex;
    private bool rightClickToResetEnabled;

    public static InventoryUIHeldItemController Instance;

    private void Awake()
    {
        Instance = this;

        heldItemRectTransform = heldItemUI.GetComponent<RectTransform>();
        heldItemImage = heldItemUI.GetComponent<Image>();

        rightClickToResetEnabled = true;

        inventoryUIManager.OnInventoryUIClosed += InventoryUIManager_OnInventoryUIClosed;
    }

    private void Update()
    {
        if (HoldingItem())
        {
            UpdateHeldItemPosition();

            if (rightClickToResetEnabled &&
                Input.GetMouseButtonDown(1) &&
                hoveredItemSlotManager.HoveredItemIndex == -1)
            {
                ResetHeldItem();
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;

        inventoryUIManager.OnInventoryUIClosed -= InventoryUIManager_OnInventoryUIClosed;
    }

    private void UpdateHeldItemPosition()
    {
        heldItemRectTransform.anchoredPosition =
            MousePositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);
    }

    public void LeftClickedItemAtIndex(Inventory clickedInventory,
        int clickedItemIndex, InventoryUIItemSlotController clickedItemSlot)
    {
        this.clickedInventory = clickedInventory;

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HoldingItem())
        {
            PlaceHeldItem(clickedItemIndex, clickedItem);
        }
        else if (clickedItem.itemData.name != "Empty")
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                clickedInventory.ConsolidateItemsToIndex(clickedItemIndex);
            }

            HoldItem(clickedInventory, clickedItemIndex,
                clickedItemSlot, clickedItem);
        }
    }

    public void RightClickedItemAtIndex(Inventory clickedInventory,
        int clickedItemIndex)
    {
        this.clickedInventory = clickedInventory;

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HoldingItem())
        {
            PlaceOneOfHeldItem(clickedItemIndex, clickedItem);
        }
    }

    public void HeldLeftClickOverItemAtIndex(Inventory clickedInventory,
        int clickedItemIndex)
    {
        if (!HoldingItem() || heldItemIndex == clickedItemIndex)
        {
            return;
        }

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (heldItem.itemData.name == clickedItem.itemData.name
            && heldItem.amount < heldItem.itemData.stackSize)
        {
            int combinedAmount = heldItem.amount + clickedItem.amount;

            if (combinedAmount <= heldItem.itemData.stackSize)
            {
                clickedInventory.RemoveItemAtIndex(clickedItemIndex);

                heldItem.amount = combinedAmount;
            }
            else
            {
                clickedInventory.SetItemAmountAtIndex(
                    combinedAmount - heldItem.itemData.stackSize, clickedItemIndex);

                heldItem.amount = heldItem.itemData.stackSize;
            }

            UpdateHeldItemUI();
        }
    }

    public void HeldRightClickOverItemAtIndex(Inventory clickedInventory,
        int clickedItemIndex)
    {
        RightClickedItemAtIndex(clickedInventory, clickedItemIndex);
    }

    private void PlaceHeldItem(int clickedItemIndex, ItemWithAmount clickedItem)
    {
        bool shouldPutHeldItemBack = clickedInventory == heldItemInventory &&
            clickedItemIndex == heldItemIndex;
        if (shouldPutHeldItemBack)
        {
            heldItemInventory.InvokeOnItemChangedAtIndexEvent(clickedItem, clickedItemIndex);
        }
        else if (clickedItem.itemData.name == heldItem.itemData.name &&
            clickedItem.amount < clickedItem.itemData.stackSize)
        {
            CombineHeldItemStackWithClickedItemStack(clickedItemIndex, clickedItem);

            return;
        }
        else if (clickedInventory == heldItemInventory)
        {
            clickedInventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
        }
        else
        {
            heldItemInventory.SwapItemsBetweenInventories(heldItemIndex,
                clickedInventory, clickedItemIndex);
        }

        HideHeldItem();
    }

    private void CombineHeldItemStackWithClickedItemStack(int clickedItemIndex,
        ItemWithAmount clickedItem)
    {
        int amountToMove = Math.Min(heldItem.amount,
            clickedItem.itemData.stackSize - clickedItem.amount);

        clickedInventory.SetItemAmountAtIndex(clickedItem.amount + amountToMove,
            clickedItemIndex);

        if (heldItem.amount == amountToMove)
        {
            heldItemInventory.RemoveItemAtIndex(heldItemIndex);

            HideHeldItem();
        }
        else
        {
            heldItem.amount -= amountToMove;

            UpdateHeldItemUI();
        }
    }

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemWithAmount clickedItem)
    {
        bool shouldPutHeldItemBack = clickedInventory == heldItemInventory &&
            clickedItemIndex == heldItemIndex;
        if (shouldPutHeldItemBack)
        {
            heldItemInventory.InvokeOnItemChangedAtIndexEvent(clickedItem, clickedItemIndex);

            HideHeldItem();
        }
        else if (clickedItem.itemData.name == heldItem.itemData.name &&
            clickedItem.amount < clickedItem.itemData.stackSize)
        {
            bool placingLastItemInHeldStack = heldItem.amount == 1;

            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);

            heldItemInventory.DecrementItemStackAtIndex(heldItemIndex);

            heldItemSlot.Clear();

            if (placingLastItemInHeldStack)
            {
                HideHeldItem();
            }
            else
            {
                RefreshHeldItem();

                UpdateHeldItemUI();
            }
        }
        else if (clickedItem.itemData.name == "Empty")
        {
            bool placingLastItemInHeldStack = heldItem.amount == 1;

            ItemWithAmount oneOfHeldItem = new ItemWithAmount(heldItem.itemData,
                1, heldItem.instanceProperties);

            clickedInventory.AddItemAtIndex(oneOfHeldItem, clickedItemIndex);

            heldItemInventory.DecrementItemStackAtIndex(heldItemIndex);

            heldItemSlot.Clear();

            if (placingLastItemInHeldStack)
            {
                HideHeldItem();
            }
            else
            {
                RefreshHeldItem();

                UpdateHeldItemUI();
            }
        }
    }

    private void HoldItem(Inventory inventory, int itemIndex,
        InventoryUIItemSlotController itemSlot, ItemWithAmount item)
    {
        heldItemInventory = inventory;
        heldItemSlot = itemSlot;

        heldItemSlot.Clear();

        heldItemIndex = itemIndex;
        heldItem = item;

        UpdateHeldItemUI();
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        ResetHeldItem();
    }

    private void RefreshHeldItem()
    {
        heldItem = heldItemInventory.GetItemList()[heldItemIndex];
    }

    private void UpdateHeldItemUI()
    {
        heldItemImage.sprite = heldItem.itemData.sprite;

        heldItemAmountText.text = heldItem.GetAmountText();

        durabilityMeter.UpdateUsingItem(heldItem);
    }

    public void ResetHeldItem()
    {
        if (HoldingItem())
        {
            heldItemSlot.UpdateUsingItem(heldItem);

            HideHeldItem();
        }
    }

    public void DecrementHeldItemStack()
    {
        heldItemInventory.DecrementItemStackAtIndex(heldItemIndex);

        if (heldItemInventory.GetItemAtIndex(heldItemIndex).amount == 0)
        {
            HideHeldItem();
        }
        else
        {
            UpdateHeldItemUI();

            heldItemSlot.Clear();
        }
    }

    public void HideHeldItem()
    {
        heldItemImage.sprite = transparentSprite;
        heldItemAmountText.text = string.Empty;

        durabilityMeter.HideMeter();
    }

    public void SetRightClickToResetEnabled(bool rightClickToResetEnabled)
    {
        this.rightClickToResetEnabled = rightClickToResetEnabled;
    }

    public bool HoldingItem() => heldItemImage.sprite != transparentSprite;

    public int GetHeldItemIndex() => heldItemIndex;

    public Inventory GetHeldItemInventory() => heldItemInventory;

    public string GetTooltipText() => heldItem.GetTooltipText();

    public string GetAlternateTooltipText() => string.Empty;
}
