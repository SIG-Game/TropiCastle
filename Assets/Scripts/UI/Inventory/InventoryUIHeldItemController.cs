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

        if (heldItem.itemDefinition.name == clickedItem.itemDefinition.name
            && heldItem.amount < heldItem.itemDefinition.stackSize)
        {
            int combinedAmount = heldItem.amount + clickedItem.amount;

            if (combinedAmount <= heldItem.itemDefinition.stackSize)
            {
                clickedInventory.RemoveItemAtIndex(clickedItemIndex);

                heldItem.amount = combinedAmount;
            }
            else
            {
                clickedInventory.SetItemAmountAtIndex(
                    combinedAmount - heldItem.itemDefinition.stackSize, clickedItemIndex);

                heldItem.amount = heldItem.itemDefinition.stackSize;
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
        else if (clickedItem.itemDefinition.name == heldItem.itemDefinition.name &&
            clickedItem.amount < clickedItem.itemDefinition.stackSize)
        {
            CombineHeldItemStackWithClickedItemStack(clickedItemIndex, clickedItem);
        }
        else if (clickedItem.itemDefinition.name == "Empty")
        {
            clickedInventory.AddItemAtIndex(heldItem, clickedItemIndex);

            HideHeldItemUI();
        }
    }

    private void CombineHeldItemStackWithClickedItemStack(
        int clickedItemIndex, ItemWithAmount clickedItem)
    {
        int amountToMove = Math.Min(heldItem.amount,
            clickedItem.itemDefinition.stackSize - clickedItem.amount);

        clickedInventory.SetItemAmountAtIndex(
            clickedItem.amount + amountToMove, clickedItemIndex);

        if (heldItem.amount == amountToMove)
        {
            HideHeldItemUI();
        }
        else
        {
            heldItem.amount -= amountToMove;

            UpdateHeldItemUI();
        }
    }

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemWithAmount clickedItem)
    {
        bool canPlaceInClickedItemStack =
            clickedItem.itemDefinition.name == heldItem.itemDefinition.name &&
            clickedItem.amount < clickedItem.itemDefinition.stackSize;
        if (canPlaceInClickedItemStack)
        {
            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);
        }
        else if (clickedItem.itemDefinition.name == "Empty")
        {
            ItemWithAmount oneOfHeldItem = new ItemWithAmount(heldItem.itemDefinition,
                1, heldItem.instanceProperties);

            clickedInventory.AddItemAtIndex(oneOfHeldItem, clickedItemIndex);
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
        heldItem = item;

        heldItemInventory.RemoveItemAtIndex(heldItemIndex);

        UpdateHeldItemUI();

        OnItemHeld();
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        ResetHeldItem();
    }

    private void UpdateHeldItemUI()
    {
        heldItemImage.sprite = heldItem.itemDefinition.sprite;

        heldItemAmountText.text = heldItem.GetAmountText();

        durabilityMeter.UpdateUsingItem(heldItem);
    }

    public void DecrementHeldItemStack()
    {
        heldItem.amount--;

        if (heldItem.amount == 0)
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
            ItemWithAmount itemAtHeldItemIndex =
                heldItemInventory.GetItemAtIndex(heldItemIndex);

            if (itemAtHeldItemIndex.itemDefinition.name == "Empty")
            {
                heldItemInventory.AddItemAtIndex(heldItem, heldItemIndex);
            }
            else
            {
                heldItemInventory.AddAmountToItemAtIndex(
                    heldItem.amount, heldItemIndex, out int amountAdded);

                int amountRemaining = heldItem.amount - amountAdded;

                if (amountRemaining != 0)
                {
                    ItemWithAmount itemWithAmountRemaining =
                        new ItemWithAmount(heldItem.itemDefinition,
                            amountRemaining, heldItem.instanceProperties);

                    heldItemInventory.AddItem(itemWithAmountRemaining);
                }
            }
        }

        HideHeldItemUI();
    }

    public void HideHeldItemUI()
    {
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
