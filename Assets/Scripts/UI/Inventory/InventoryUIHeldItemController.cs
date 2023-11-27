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

    public event Action OnItemHeld = () => {};
    public event Action OnHidden = () => {};

    public bool RightClickToResetEnabled
    {
        private get => rightClickToResetEnabled;
        set
        {
            rightClickToResetEnabled = value;
        }
    }

    private ItemStack HeldItem
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
    private ItemStack heldItem;
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
            heldItemRectTransform.anchoredPosition =
                MousePositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);

            if (rightClickToResetEnabled &&
                Input.GetMouseButtonDown(1) &&
                hoveredItemSlotManager.HoveredItemIndex == -1)
            {
                PutHeldItemBack();
            }
        }
    }

    private void OnDestroy()
    {
        inventoryUIManager.OnInventoryUIClosed -= InventoryUIManager_OnInventoryUIClosed;
    }

    public void LeftClickedItemAtIndex(
        Inventory clickedInventory, int clickedItemIndex, bool itemPlacementEnabled)
    {
        this.clickedInventory = clickedInventory;

        ItemStack clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HoldingItem())
        {
            if (itemPlacementEnabled)
            {
                PlaceHeldItem(clickedItemIndex, clickedItem);
            }
        }
        else if (!clickedItem.itemDefinition.IsEmpty())
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

        ItemStack clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

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

        ItemStack clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

        if (HeldItem.itemDefinition.name == clickedItem.itemDefinition.name
            && HeldItem.amount < HeldItem.itemDefinition.StackSize)
        {
            int combinedAmount = HeldItem.amount + clickedItem.amount;

            if (combinedAmount <= HeldItem.itemDefinition.StackSize)
            {
                clickedInventory.RemoveItemAtIndex(clickedItemIndex);

                HeldItem.amount = combinedAmount;
            }
            else
            {
                clickedInventory.SetItemAmountAtIndex(
                    combinedAmount - HeldItem.itemDefinition.StackSize, clickedItemIndex);

                HeldItem.amount = HeldItem.itemDefinition.StackSize;
            }

            UpdateHeldItemUI();
        }
    }

    public void HeldRightClickOverItemAtIndex(Inventory clickedInventory, int clickedItemIndex)
    {
        RightClickedItemAtIndex(clickedInventory, clickedItemIndex);
    }

    private void PlaceHeldItem(int clickedItemIndex, ItemStack clickedItem)
    {
        if (clickedInventory == heldItemInventory && clickedItemIndex == heldItemIndex)
        {
            PutHeldItemBack();
        }
        else if (clickedItem.itemDefinition.name == HeldItem.itemDefinition.name &&
            clickedItem.amount < clickedItem.itemDefinition.StackSize)
        {
            CombineHeldItemStackWithClickedItemStack(clickedItemIndex, clickedItem);
        }
        else if (clickedItem.itemDefinition.IsEmpty())
        {
            clickedInventory.AddItemAtEmptyItemIndex(HeldItem, clickedItemIndex);

            HideHeldItemUI();
        }
    }

    private void CombineHeldItemStackWithClickedItemStack(
        int clickedItemIndex, ItemStack clickedItem)
    {
        int amountToMove = Math.Min(HeldItem.amount,
            clickedItem.itemDefinition.StackSize - clickedItem.amount);

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

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemStack clickedItem)
    {
        bool canPlaceInClickedItemStack =
            clickedItem.itemDefinition.name == HeldItem.itemDefinition.name &&
            clickedItem.amount < clickedItem.itemDefinition.StackSize;
        if (canPlaceInClickedItemStack)
        {
            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);
        }
        else if (clickedItem.itemDefinition.IsEmpty())
        {
            ItemStack oneOfHeldItem = HeldItem.GetCopyWithAmount(1);

            clickedInventory.AddItemAtEmptyItemIndex(oneOfHeldItem, clickedItemIndex);
        }
        else
        {
            return;
        }

        DecrementHeldItemStack();
    }

    private void HoldItem(Inventory inventory, int itemIndex, ItemStack item)
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
        PutHeldItemBack();
    }

    private void UpdateHeldItemUI()
    {
        if (HoldingItem())
        {
            heldItemImage.sprite = HeldItem.itemDefinition.Sprite;
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

    public void PutHeldItemBack()
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

    public bool HoldingItem() => HeldItem != null;

    public string GetTooltipText() => HeldItem.GetTooltipText();
}
