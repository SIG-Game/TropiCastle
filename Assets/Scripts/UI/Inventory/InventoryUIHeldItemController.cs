using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIHeldItemController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject heldItemUI;
    [SerializeField] private GameObject canvas;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Sprite transparentSprite;

    private Inventory inventory;
    private GraphicRaycaster graphicRaycaster;
    private RectTransform canvasRectTransform;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private int heldItemIndex;

    private void Awake()
    {
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();

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
            MouseCanvasPositionHelper.GetMouseCanvasPosition(canvasRectTransform);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, raycastResults);

        if (raycastResults.Count == 0)
        {
            return;
        }

        int clickedItemIndex = raycastResults[0].gameObject.transform.GetSiblingIndex();
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

    private bool HoldingItem() => heldItemImage.sprite != transparentSprite;

    private void HideHeldItem()
    {
        heldItemImage.sprite = transparentSprite;
    }

    private Vector2 ClampPositionWithCenterAnchorToCanvas(Vector2 position) =>
        new Vector2(ClampValueWithCenterAnchorToCanvasWidth(position.x),
            ClampValueWithCenterAnchorToCanvasHeight(position.y));

    private float ClampValueWithCenterAnchorToCanvasWidth(float value) =>
        ClampValueWithCenterAnchorToLimit(value, canvasRectTransform.rect.width);

    private float ClampValueWithCenterAnchorToCanvasHeight(float value) =>
        ClampValueWithCenterAnchorToLimit(value, canvasRectTransform.rect.height);

    private float ClampValueWithCenterAnchorToLimit(float value, float limit)
    {
        float halfLimit = limit / 2f;
        return Mathf.Clamp(value, -halfLimit, halfLimit);
    }
}
