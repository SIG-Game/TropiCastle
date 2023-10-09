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

    private ItemWithAmount HeldItem
    {
        get => heldItem;
        set
        {
            heldItem = value;

            if (heldItem == null)
            {
                heldItemInventory = null;
                heldItemIndex = null;
            }

            UpdateHeldItemUI();
        }
    }

    private Inventory clickedInventory;
    private Inventory heldItemInventory;
    private ItemWithAmount heldItem;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private int? heldItemIndex;
    private bool rightClickToResetEnabled;

    private void Awake()
    {
        heldItemRectTransform = GetComponent<RectTransform>();
        heldItemImage = GetComponent<Image>();

        heldItemIndex = null;
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

    public void LeftClickedItemAtIndex(Inventory clickedInventory, int clickedItemIndex)
    {
        this.clickedInventory = clickedInventory;

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HoldingItem())
        {
            PlaceHeldItem(clickedItemIndex, clickedItem);
        }
        else if (clickedItem.itemDefinition.name != "Empty")
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                clickedInventory.ConsolidateItemsToIndex(clickedItemIndex);
            }

            HoldItem(clickedInventory, clickedItemIndex, clickedItem);
        }
    }

    public void RightClickedItemAtIndex(Inventory clickedInventory, int clickedItemIndex)
    {
        this.clickedInventory = clickedInventory;

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HoldingItem())
        {
            PlaceOneOfHeldItem(clickedItemIndex, clickedItem);
        }
    }

    public void HeldLeftClickOverItemAtIndex(Inventory clickedInventory, int clickedItemIndex)
    {
        if (!HoldingItem())
        {
            return;
        }

        ItemWithAmount clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HeldItem.itemDefinition.name == clickedItem.itemDefinition.name
            && HeldItem.amount < HeldItem.itemDefinition.stackSize)
        {
            int combinedAmount = HeldItem.amount + clickedItem.amount;

            if (combinedAmount <= HeldItem.itemDefinition.stackSize)
            {
                clickedInventory.RemoveItemAtIndex(clickedItemIndex);

                HeldItem.amount = combinedAmount;
            }
            else
            {
                clickedInventory.SetItemAmountAtIndex(
                    combinedAmount - HeldItem.itemDefinition.stackSize, clickedItemIndex);

                HeldItem.amount = HeldItem.itemDefinition.stackSize;
            }

            UpdateHeldItemUI();
        }
    }

    public void HeldRightClickOverItemAtIndex(Inventory clickedInventory, int clickedItemIndex)
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
        else if (clickedItem.itemDefinition.name == HeldItem.itemDefinition.name &&
            clickedItem.amount < clickedItem.itemDefinition.stackSize)
        {
            CombineHeldItemStackWithClickedItemStack(clickedItemIndex, clickedItem);
        }
        else if (clickedItem.itemDefinition.name == "Empty")
        {
            clickedInventory.AddItemAtEmptyItemIndex(HeldItem, clickedItemIndex);

            HideHeldItemUI();
        }
    }

    private void CombineHeldItemStackWithClickedItemStack(
        int clickedItemIndex, ItemWithAmount clickedItem)
    {
        int amountToMove = Math.Min(HeldItem.amount,
            clickedItem.itemDefinition.stackSize - clickedItem.amount);

        clickedInventory.SetItemAmountAtIndex(
            clickedItem.amount + amountToMove, clickedItemIndex);

        if (HeldItem.amount == amountToMove)
        {
            HideHeldItemUI();
        }
        else
        {
            HeldItem.amount -= amountToMove;

            UpdateHeldItemUI();
        }
    }

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemWithAmount clickedItem)
    {
        bool canPlaceInClickedItemStack =
            clickedItem.itemDefinition.name == HeldItem.itemDefinition.name &&
            clickedItem.amount < clickedItem.itemDefinition.stackSize;
        if (canPlaceInClickedItemStack)
        {
            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);
        }
        else if (clickedItem.itemDefinition.name == "Empty")
        {
            ItemWithAmount oneOfHeldItem = new ItemWithAmount(
                HeldItem.itemDefinition, 1, HeldItem.instanceProperties);

            clickedInventory.AddItemAtEmptyItemIndex(oneOfHeldItem, clickedItemIndex);
        }
        else
        {
            return;
        }

        DecrementHeldItemStack();
    }

    private void HoldItem(Inventory inventory, int itemIndex, ItemWithAmount item)
    {
        heldItemInventory = inventory;
        heldItemIndex = itemIndex;
        HeldItem = item;

        heldItemInventory.RemoveItemAtIndex(heldItemIndex.Value);

        UpdateHeldItemUI();

        OnItemHeld();
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        ResetHeldItem();
    }

    private void UpdateHeldItemUI()
    {
        if (HeldItem != null)
        {
            heldItemImage.sprite = HeldItem.itemDefinition.sprite;
            heldItemAmountText.text = HeldItem.GetAmountText();
            durabilityMeter.UpdateUsingItem(HeldItem);
        }
        else
        {
            heldItemImage.sprite = transparentSprite;
            heldItemAmountText.text = string.Empty;
            durabilityMeter.HideMeter();

            OnHidden();
        }
    }

    public void DecrementHeldItemStack()
    {
        HeldItem.amount--;

        if (HeldItem.amount == 0)
        {
            HideHeldItemUI();
        }
        else
        {
            UpdateHeldItemUI();
        }
    }

    public void ResetHeldItem()
    {
        if (HoldingItem())
        {
            heldItemInventory.AddItemAtIndex(HeldItem, heldItemIndex.Value);
        }

        HideHeldItemUI();
    }

    public void HideHeldItemUI()
    {
        HeldItem = null;
    }

    public void SetRightClickToResetEnabled(bool rightClickToResetEnabled)
    {
        this.rightClickToResetEnabled = rightClickToResetEnabled;
    }

    public bool HoldingItem() => HeldItem != null;

    public int GetHeldItemIndex() => heldItemIndex.Value;

    public Inventory GetHeldItemInventory() => heldItemInventory;

    public string GetTooltipText() => HeldItem.GetTooltipText();
}
