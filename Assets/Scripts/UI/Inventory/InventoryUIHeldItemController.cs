using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour
{
    [SerializeField] private GameObject heldItemUI;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Sprite transparentSprite;

    private Inventory inventory;
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

        inventory = inventoryUIController.GetInventory();

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
            MouseCanvasPositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);
    }

    public void LeftClickedItemAtIndex(int clickedItemIndex)
    {
        ItemScriptableObject clickedItemData = inventory.GetItemAtIndex(clickedItemIndex).itemData;

        if (HoldingItem())
        {
            PlaceHeldItem(clickedItemIndex, clickedItemData);
        }
        else if (clickedItemData.name != "Empty")
        {
            HoldItem(clickedItemIndex, clickedItemData);
        }
    }

    private void PlaceHeldItem(int itemIndex, ItemScriptableObject itemData)
    {
        bool shouldPutHeldItemBack = itemIndex == heldItemIndex;
        if (shouldPutHeldItemBack)
        {
            inventoryUIController.SetSpriteAtSlotIndex(itemData.sprite, itemIndex);
        }
        else
        {
            inventory.SwapItemsAt(heldItemIndex, itemIndex);
        }

        HideHeldItem();
    }

    private void HoldItem(int itemIndex, ItemScriptableObject itemData)
    {
        inventoryUIController.SetSpriteAtSlotIndex(transparentSprite, itemIndex);

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
            ItemWithAmount heldItem = inventory.GetItemAtIndex(heldItemIndex);

            inventoryUIController.SetSpriteAtSlotIndex(heldItem.itemData.sprite, heldItemIndex);

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
