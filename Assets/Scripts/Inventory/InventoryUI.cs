using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IPointerClickHandler
{
    private Inventory inventory;
    private GraphicRaycaster graphicRaycaster;
    private RectTransform canvasRectTransform;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private const int hotbarSize = 10;
    private int hotbarItemIndex = 0;
    private bool holdingItem = false;
    private int heldItemIndex;

    public Sprite transparentSprite;
    public Transform hotbarItemSlotContainer;
    public Transform itemSlotContainer;
    public GameObject heldItem;
    public GameObject canvas;

    void Awake()
    {
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        heldItemRectTransform = heldItem.GetComponent<RectTransform>();
        heldItemImage = heldItem.GetComponent<Image>();
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.ChangedItemAt = Inventory_ChangedItemAt;
    }

    private void Inventory_ChangedItemAt(int index)
    {
        List<ItemWithAmount> inventoryItemList = inventory.GetItemList();
        ItemWithAmount item = inventoryItemList[index];

        SetSpriteAtSlotIndex(index, item.itemData.sprite);
    }

    private void SetSpriteAtSlotIndex(int slotIndex, Sprite newSprite)
    {
        if (slotIndex < hotbarSize)
        {
            hotbarItemSlotContainer.GetChild(slotIndex).GetChild(0).GetComponent<Image>().sprite = newSprite;
        }

        itemSlotContainer.GetChild(slotIndex).GetChild(0).GetComponent<Image>().sprite = newSprite;
    }

    public void selectHotbarItem(int hotbarItemIndex)
    {
        hotbarItemSlotContainer.GetChild(this.hotbarItemIndex).GetComponent<Image>().color = new Color32(173, 173, 173, 255);
        this.hotbarItemIndex = hotbarItemIndex;
        hotbarItemSlotContainer.GetChild(this.hotbarItemIndex).GetComponent<Image>().color = new Color32(140, 140, 140, 255);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        if (results.Count > 0)
        {
            int clickedItemIndex = results[0].gameObject.transform.GetSiblingIndex();
            ItemWithAmount clickedItem = inventory.GetItemList()[clickedItemIndex];

            if (holdingItem)
            {
                if (clickedItemIndex == heldItemIndex)
                {
                    // Put held item back
                    SetSpriteAtSlotIndex(clickedItemIndex, clickedItem.itemData.sprite);
                }
                else
                {
                    inventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
                }

                heldItemImage.sprite = transparentSprite;
                holdingItem = false;
            }
            else if (clickedItem.itemData.name != "Empty")
            {
                // Hold clicked item
                SetSpriteAtSlotIndex(clickedItemIndex, transparentSprite);

                heldItemIndex = clickedItemIndex;
                heldItemImage.sprite = clickedItem.itemData.sprite;
                holdingItem = true;
            }
        }
    }

    public void ResetHeldItem()
    {
        if (holdingItem)
        {
            ItemWithAmount heldItem = inventory.GetItemList()[heldItemIndex];

            SetSpriteAtSlotIndex(heldItemIndex, heldItem.itemData.sprite);

            heldItemImage.sprite = transparentSprite;

            holdingItem = false;
        }
    }

    private void Update()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0f, canvasRectTransform.rect.width);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0f, canvasRectTransform.rect.height);
        heldItemRectTransform.anchoredPosition = anchoredPosition;
    }
}
