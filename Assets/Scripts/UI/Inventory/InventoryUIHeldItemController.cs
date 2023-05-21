using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour
{
    [SerializeField] private GameObject heldItemUI;
    [SerializeField] private TextMeshProUGUI heldItemAmountText;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Sprite transparentSprite;

    private Inventory clickedInventory;
    private Inventory heldItemInventory;
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
        int clickedItemIndex, InventoryUIItemSlotController clickedItemSlot)
    {
        this.clickedInventory = clickedInventory;

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HoldingItem())
        {
            PlaceOneOfHeldItem(clickedItemIndex, clickedItem);
        }
    }

    private void PlaceHeldItem(int itemIndex, ItemWithAmount clickedItem)
    {
        bool shouldPutHeldItemBack = clickedInventory == heldItemInventory &&
            itemIndex == heldItemIndex;
        if (shouldPutHeldItemBack)
        {
            inventoryUIController.UpdateSlotAtIndexUsingItem(itemIndex, clickedItem);
        }
        else if (clickedInventory == heldItemInventory)
        {
            clickedInventory.SwapItemsAt(heldItemIndex, itemIndex);
        }
        else
        {
            heldItemInventory.SwapItemsBetweenInventories(heldItemIndex,
                clickedInventory, itemIndex);
        }

        HideHeldItem();
    }

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemWithAmount clickedItem)
    {
        ItemWithAmount heldItem = heldItemInventory.GetItemList()[heldItemIndex];

        bool shouldPutHeldItemBack = clickedInventory == heldItemInventory &&
            clickedItemIndex == heldItemIndex;
        if (shouldPutHeldItemBack)
        {
            inventoryUIController.UpdateSlotAtIndexUsingItem(clickedItemIndex, clickedItem);

            HideHeldItem();
        }
        else if (clickedItem.itemData.name == heldItem.itemData.name &&
            clickedItem.amount < clickedItem.itemData.stackSize)
        {
            bool hideHeldItem = heldItem.amount == 1;

            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);

            heldItemInventory.DecrementItemStackAtIndex(heldItemIndex);

            EmptyHeldItemSlotUI();

            if (hideHeldItem)
            {
                HideHeldItem();
            }
            else
            {
                heldItem = heldItemInventory.GetItemList()[heldItemIndex];

                UpdateHeldItemUI(heldItem);
            }
        }
        else if (clickedItem.itemData.name == "Empty")
        {
            bool hideHeldItem = heldItem.amount == 1;

            ItemWithAmount oneOfHeldItem = new ItemWithAmount(heldItem.itemData,
                1, heldItem.instanceProperties);

            clickedInventory.AddItemAtIndex(oneOfHeldItem, clickedItemIndex);

            heldItemInventory.DecrementItemStackAtIndex(heldItemIndex);

            EmptyHeldItemSlotUI();

            if (hideHeldItem)
            {
                HideHeldItem();
            }
            else
            {
                heldItem = heldItemInventory.GetItemList()[heldItemIndex];

                UpdateHeldItemUI(heldItem);
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

        UpdateHeldItemUI(item);

        tooltipTextWithPriority = new Tooltip(
            InventoryUITooltipController.GetItemTooltipText(item.itemData), 1);
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
        heldItemSlot.SetAmountText(0);
        heldItemSlot.SetItemInstanceProperties(null);
    }

    private void UpdateHeldItemUI(ItemWithAmount heldItem)
    {
        heldItemImage.sprite = heldItem.itemData.sprite;

        if (heldItem.amount > 1)
        {
            heldItemAmountText.text = heldItem.amount.ToString();
        }
    }

    private void ResetHeldItem()
    {
        if (HoldingItem())
        {
            ItemWithAmount heldItem = heldItemInventory.GetItemAtIndex(heldItemIndex);

            heldItemSlot.UpdateUsingItem(heldItem);

            HideHeldItem();
        }
    }

    public void HideHeldItem()
    {
        heldItemImage.sprite = transparentSprite;
        heldItemAmountText.text = string.Empty;

        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);

        OnStoppedHoldingItem();
    }

    public bool HoldingItem() => heldItemImage.sprite != transparentSprite;

    public int GetHeldItemIndex() => heldItemIndex;
}
