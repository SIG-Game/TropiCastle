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

            if (Input.GetMouseButtonDown(1))
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

    private void HoldItem(Inventory inventory, int itemIndex,
        InventoryUIItemSlotController itemSlot, ItemWithAmount item)
    {
        heldItemInventory = inventory;
        heldItemSlot = itemSlot;

        heldItemSlot.SetSprite(transparentSprite);
        heldItemSlot.SetAmountText(0);
        heldItemSlot.SetItemInstanceProperties(null);

        heldItemIndex = itemIndex;
        heldItemImage.sprite = item.itemData.sprite;

        if (item.amount > 1)
        {
            heldItemAmountText.text = item.amount.ToString();
        }

        tooltipTextWithPriority = new Tooltip(
            InventoryUITooltipController.GetItemTooltipText(item.itemData), 1);
        InventoryUITooltipController.Instance.AddTooltipTextWithPriority(tooltipTextWithPriority);

        OnStartedHoldingItem();
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        ResetHeldItem();
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
