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
    private int heldItemIndex;

    private void Awake()
    {
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
        }
    }

    private void OnDestroy()
    {
        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    private void UpdateHeldItemPosition()
    {
        heldItemRectTransform.anchoredPosition =
            MouseCanvasPositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);
    }

    public void ClickedItemAtIndex(int clickedItemIndex)
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
            inventoryUIController.SetInventorySpriteAtSlotIndex(itemData.sprite, itemIndex);
        }
        else
        {
            inventory.SwapItemsAt(heldItemIndex, itemIndex);
        }

        HideHeldItem();
    }

    private void HoldItem(int itemIndex, ItemScriptableObject itemData)
    {
        inventoryUIController.SetInventorySpriteAtSlotIndex(transparentSprite, itemIndex);

        heldItemIndex = itemIndex;
        heldItemImage.sprite = itemData.sprite;
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

            inventoryUIController.SetInventorySpriteAtSlotIndex(heldItem.itemData.sprite, heldItemIndex);

            HideHeldItem();
        }
    }

    private void HideHeldItem()
    {
        heldItemImage.sprite = transparentSprite;
    }

    public bool HoldingItem() => heldItemImage.sprite != transparentSprite;
}
