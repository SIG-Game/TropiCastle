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

    private ItemStack? HeldItem
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
    private ItemStack? heldItem;
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
        else if (!clickedItem.ItemDefinition.IsEmpty())
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                clickedInventory.ConsolidateItemsToIndex(clickedItemIndex);
            }

            clickedItem = clickedInventory.GetItemAtIndex(clickedItemIndex);

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

        if (HeldItem.Value.ItemDefinition.name == clickedItem.ItemDefinition.name
            && HeldItem.Value.Amount < HeldItem.Value.ItemDefinition.StackSize)
        {
            int combinedAmount = HeldItem.Value.Amount + clickedItem.Amount;

            if (combinedAmount <= HeldItem.Value.ItemDefinition.StackSize)
            {
                clickedInventory.RemoveItemAtIndex(clickedItemIndex);

                HeldItem = HeldItem.Value.GetCopyWithAmount(combinedAmount);
            }
            else
            {
                clickedInventory.SetItemAmountAtIndex(
                    combinedAmount - HeldItem.Value.ItemDefinition.StackSize,
                    clickedItemIndex);

                HeldItem = HeldItem.Value.GetCopyWithAmount(
                    HeldItem.Value.ItemDefinition.StackSize);
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
        else if (clickedItem.ItemDefinition.name == HeldItem.Value.ItemDefinition.name &&
            clickedItem.Amount < clickedItem.ItemDefinition.StackSize)
        {
            CombineHeldItemStackWithClickedItemStack(clickedItemIndex, clickedItem);
        }
        else if (clickedItem.ItemDefinition.IsEmpty())
        {
            clickedInventory.AddItemAtEmptyItemIndex(HeldItem.Value, clickedItemIndex);

            HideHeldItemUI();
        }
    }

    private void CombineHeldItemStackWithClickedItemStack(
        int clickedItemIndex, ItemStack clickedItem)
    {
        int amountToMove = Math.Min(HeldItem.Value.Amount,
            clickedItem.ItemDefinition.StackSize - clickedItem.Amount);

        clickedInventory.SetItemAmountAtIndex(
            clickedItem.Amount + amountToMove, clickedItemIndex);

        if (HeldItem.Value.Amount == amountToMove)
        {
            HideHeldItemUI();
        }
        else
        {
            HeldItem = HeldItem.Value.GetCopyWithAmount(
                HeldItem.Value.Amount - amountToMove);

            UpdateHeldItemUI();
        }
    }

    private void PlaceOneOfHeldItem(int clickedItemIndex, ItemStack clickedItem)
    {
        bool canPlaceInClickedItemStack =
            clickedItem.ItemDefinition.name == HeldItem.Value.ItemDefinition.name &&
            clickedItem.Amount < clickedItem.ItemDefinition.StackSize;
        if (canPlaceInClickedItemStack)
        {
            clickedInventory.IncrementItemStackAtIndex(clickedItemIndex);
        }
        else if (clickedItem.ItemDefinition.IsEmpty())
        {
            ItemStack oneOfHeldItem = HeldItem.Value.GetCopyWithAmount(1);

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
            heldItemImage.sprite = HeldItem.Value.ItemDefinition.Sprite;
            heldItemAmountText.text = HeldItem.Value.GetAmountText();
            durabilityMeter.UpdateUsingItem(HeldItem.Value);
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
        HeldItem = HeldItem.Value.GetCopyWithAmount(
            HeldItem.Value.Amount - 1);

        if (HeldItem.Value.Amount == 0)
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
            heldItemInventory.AddItemAtIndex(HeldItem.Value, heldItemIndex.Value);
        }

        HideHeldItemUI();
    }

    public void HideHeldItemUI()
    {
        HeldItem = null;
    }

    public bool HoldingItem() => HeldItem != null;

    public string GetTooltipText() => HeldItem.Value.GetTooltipText();
}
