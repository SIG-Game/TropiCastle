using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour
{
    [SerializeField] private GameObject heldItemUI;
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

        ItemScriptableObject clickedItemData = clickedItem.itemData;

        if (HoldingItem())
        {
            PlaceHeldItem(clickedItemIndex, clickedItem);
        }
        else if (clickedItemData.name != "Empty")
        {
            HoldItem(clickedInventory, clickedItemIndex,
                clickedItemSlot, clickedItemData);
        }
    }

    private void PlaceHeldItem(int itemIndex, ItemWithAmount clickedItem)
    {
        bool shouldPutHeldItemBack = clickedInventory == heldItemInventory &&
            itemIndex == heldItemIndex;
        if (shouldPutHeldItemBack)
        {
            inventoryUIController.SetSpriteAtSlotIndex(
                clickedItem.itemData.sprite, itemIndex);
            inventoryUIController.SetAmountTextAtSlotIndex(
                clickedItem.amount, itemIndex);
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
        InventoryUIItemSlotController itemSlot, ItemScriptableObject itemData)
    {
        heldItemInventory = inventory;
        heldItemSlot = itemSlot;

        heldItemSlot.SetSprite(transparentSprite);
        heldItemSlot.SetAmountText(0);

        heldItemIndex = itemIndex;
        heldItemImage.sprite = itemData.sprite;

        tooltipTextWithPriority = new Tooltip(
            InventoryUITooltipController.GetItemTooltipText(itemData), 1);
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

            heldItemSlot.SetSprite(heldItem.itemData.sprite);
            heldItemSlot.SetAmountText(heldItem.amount);

            HideHeldItem();
        }
    }

    public void HideHeldItem()
    {
        heldItemImage.sprite = transparentSprite;

        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);

        OnStoppedHoldingItem();
    }

    public bool HoldingItem() => heldItemImage.sprite != transparentSprite;

    public int GetHeldItemIndex() => heldItemIndex;
}
