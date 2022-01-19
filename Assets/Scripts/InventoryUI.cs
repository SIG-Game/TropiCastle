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
        List<Item> inventoryItemList = inventory.GetItemList();
        Item item = inventoryItemList[index];

        if (index < hotbarSize)
            hotbarItemSlotContainer.GetChild(index).GetChild(0).GetComponent<Image>().sprite = item.GetSprite();

        itemSlotContainer.GetChild(index).GetChild(0).GetComponent<Image>().sprite = item.GetSprite();
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
            Item clickedItem = inventory.GetItemList()[clickedItemIndex];

            if (holdingItem)
            {
                if (clickedItemIndex == heldItemIndex)
                {
                    // Put held item back
                    if (clickedItemIndex < hotbarSize)
                        hotbarItemSlotContainer.GetChild(clickedItemIndex).GetChild(0).GetComponent<Image>().sprite =
                            clickedItem.GetSprite();

                    itemSlotContainer.GetChild(clickedItemIndex).GetChild(0).GetComponent<Image>().sprite =
                        clickedItem.GetSprite();
                }
                else
                {
                    inventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
                }

                heldItemImage.sprite = Item.GetSprite(Item.ItemType.Empty);
                holdingItem = false;
            }
            else if (clickedItem.itemType != Item.ItemType.Empty)
            {
                // Hold clicked item
                if (clickedItemIndex < hotbarSize)
                    hotbarItemSlotContainer.GetChild(clickedItemIndex).GetChild(0).GetComponent<Image>().sprite =
                        Item.GetSprite(Item.ItemType.Empty);

                itemSlotContainer.GetChild(clickedItemIndex).GetChild(0).GetComponent<Image>().sprite =
                    Item.GetSprite(Item.ItemType.Empty);

                heldItemIndex = clickedItemIndex;
                heldItemImage.sprite = Item.GetSprite(clickedItem.itemType);
                holdingItem = true;
            }
        }
    }

    public void ResetHeldItem()
    {
        if (holdingItem)
        {
            Item heldItem = inventory.GetItemList()[heldItemIndex];

            if (heldItemIndex < hotbarSize)
                hotbarItemSlotContainer.GetChild(heldItemIndex).GetChild(0).GetComponent<Image>().sprite = heldItem.GetSprite();

            itemSlotContainer.GetChild(heldItemIndex).GetChild(0).GetComponent<Image>().sprite = heldItem.GetSprite();

            heldItemImage.sprite = Item.GetSprite(Item.ItemType.Empty);

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
