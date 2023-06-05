using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour
{
    [SerializeField] private GameObject heldItemUI;
    [SerializeField] private TextMeshProUGUI heldItemAmountText;
    [SerializeField] private ItemDurabilityMeterController durabilityMeter;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Sprite transparentSprite;

    private Inventory clickedInventory;
    private Inventory heldItemInventory;
    private ItemWithAmount heldItem;
    private InventoryUIItemSlotController heldItemSlot;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private Tooltip tooltipTextWithPriority;
    private int heldItemIndex;

    public static InventoryUIHeldItemController Instance;

    public static event Action OnStartedHoldingItem = delegate { };
    public static event Action OnStoppedHoldingItem = delegate { };

    private void Awake()
    {
        Instance = this;

        heldItemRectTransform = heldItemUI.GetComponent<RectTransform>();
        heldItemImage = heldItemUI.GetComponent<Image>();

        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void Update()
    {
        if (HoldingItem())
        {
            UpdateHeldItemPosition();

            if (Input.GetMouseButtonDown(1) && inventoryUIController.HoveredItemIndex == -1)
            {
                ResetHeldItem();
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;

        OnStartedHoldingItem = delegate { };
        OnStoppedHoldingItem = delegate { };

        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
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

        clickedItem.amount += amountToMove;

        clickedInventory.InvokeOnItemChangedAtIndexEvent(clickedItem, clickedItemIndex);

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

            EmptyHeldItemSlotUI();

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

            EmptyHeldItemSlotUI();

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

        EmptyHeldItemSlotUI();

        heldItemIndex = itemIndex;
        heldItem = item;

        UpdateHeldItemUI();

        tooltipTextWithPriority = new Tooltip(
            InventoryUITooltipController.GetItemTooltipText(item), 1);
        InventoryUITooltipController.Instance.AddTooltipTextWithPriority(tooltipTextWithPriority);

        OnStartedHoldingItem();
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        ResetHeldItem();
    }

    private void EmptyHeldItemSlotUI()
    {
        heldItemSlot.SetSprite(transparentSprite);
        heldItemSlot.SetAmountText(string.Empty);
        heldItemSlot.HideDurabilityMeter();
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

    private void ResetHeldItem()
    {
        if (HoldingItem())
        {
            heldItemSlot.UpdateUsingItem(heldItem);

            HideHeldItem();
        }
    }

    public void HideHeldItem()
    {
        heldItemImage.sprite = transparentSprite;
        heldItemAmountText.text = string.Empty;

        durabilityMeter.HideMeter();

        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);

        OnStoppedHoldingItem();
    }

    public bool HoldingItem() => heldItemImage.sprite != transparentSprite;

    public int GetHeldItemIndex() => heldItemIndex;
}
