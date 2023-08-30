using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour, IElementWithTooltip
{
    [SerializeField] private TextMeshProUGUI heldItemAmountText;
    [SerializeField] private ItemDurabilityMeterController durabilityMeter;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;
    [SerializeField] private Sprite transparentSprite;

    public event Action OnItemHeld = delegate { };
    public event Action OnHidden = delegate { };

    private Inventory clickedInventory;
    private Inventory heldItemInventory;
    private ItemWithAmount heldItem;
    private InventoryUIItemSlotController heldItemSlot;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private int heldItemIndex;
    private bool rightClickToResetEnabled;

    private void Awake()
    {
        heldItemRectTransform = GetComponent<RectTransform>();
        heldItemImage = GetComponent<Image>();

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
        bool putHeldItemBack = clickedInventory == heldItemInventory &&
            clickedItemIndex == heldItemIndex;
        if (putHeldItemBack)
        {
            ResetHeldItem();
        }
        else if (clickedItem.itemData.name == heldItem.itemData.name &&
            clickedItem.amount < clickedItem.itemData.stackSize)
        {
            CombineHeldItemStackWithClickedItemStack(clickedItemIndex, clickedItem);
        }
        else if (clickedInventory == heldItemInventory)
        {
            HideHeldItemUI();

            clickedInventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
        }
        else
        {
            HideHeldItemUI();

            heldItemInventory.SwapItemsBetweenInventories(heldItemIndex,
                clickedInventory, clickedItemIndex);
        }
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
            HideHeldItemUI();

            heldItemInventory.RemoveItemAtIndex(heldItemIndex);
        }
        else
        {
            heldItem.amount -= amountToMove;

            UpdateHeldItemUI();
        }
    }

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemWithAmount clickedItem)
    {
        bool putHeldItemBack = clickedInventory == heldItemInventory &&
            clickedItemIndex == heldItemIndex;
        if (putHeldItemBack)
        {
            ResetHeldItem();

            return;
        }

        bool canPlaceInClickedItemStack =
            clickedItem.itemData.name == heldItem.itemData.name &&
            clickedItem.amount < clickedItem.itemData.stackSize;
        bool clickedEmptyItem = clickedItem.itemData.name == "Empty";
        if (canPlaceInClickedItemStack)
        {
            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);
        }
        else if (clickedEmptyItem)
        {
            ItemWithAmount oneOfHeldItem = new ItemWithAmount(heldItem.itemData,
                1, heldItem.instanceProperties);

            clickedInventory.AddItemAtIndex(oneOfHeldItem, clickedItemIndex);
        }
        else
        {
            return;
        }

        DecrementHeldItemStack();
    }

    private void HoldItem(Inventory inventory, int itemIndex,
        InventoryUIItemSlotController itemSlot, ItemWithAmount item)
    {
        heldItemInventory = inventory;
        heldItemSlot = itemSlot;

        heldItemSlot.Clear();

        heldItemSlot.DisableUpdatingUsingInventory();

        heldItemIndex = itemIndex;
        heldItem = item;

        UpdateHeldItemUI();

        OnItemHeld();
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        ResetHeldItem();
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
            heldItemSlot.UpdateUsingItem(heldItem, false);

            HideHeldItemUI();
        }
    }

    public void DecrementHeldItemStack()
    {
        bool willEmptyHeldItemStack = heldItem.amount == 1;

        heldItemInventory.DecrementItemStackAtIndex(heldItemIndex);

        if (willEmptyHeldItemStack)
        {
            HideHeldItemUI();
        }
        else
        {
            UpdateHeldItemUI();
        }
    }

    public void HideHeldItemUI()
    {
        if (HoldingItem())
        {
            heldItemSlot.EnableUpdatingUsingInventory();
        }

        heldItemImage.sprite = transparentSprite;
        heldItemAmountText.text = string.Empty;

        durabilityMeter.HideMeter();

        OnHidden();
    }

    public void SetRightClickToResetEnabled(bool rightClickToResetEnabled)
    {
        this.rightClickToResetEnabled = rightClickToResetEnabled;
    }

    public bool HoldingItem() => heldItemImage.sprite != transparentSprite;

    public int GetHeldItemIndex() => heldItemIndex;

    public Inventory GetHeldItemInventory() => heldItemInventory;

    public string GetTooltipText() => heldItem.GetTooltipText();
}
