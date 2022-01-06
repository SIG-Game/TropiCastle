using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IPointerClickHandler
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private GraphicRaycaster graphicRaycaster;
    private RectTransform canvasRectTransform;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private const int hotbarSize = 10;
    private int hotbarItemIndex = 0;
    private bool holdingItem = false;
    private int heldItemIndex;

    public Transform hotbarItemSlotContainer;
    public GameObject heldItem;

    public GameObject player;
    public GameObject canvas;

    void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
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
            hotbarItemSlotContainer.GetChild(index).GetChild(0).gameObject.GetComponent<Image>().sprite = item.GetSprite();

        itemSlotContainer.GetChild(index).GetChild(0).GetComponent<Image>().sprite = item.GetSprite();
    }

    public void selectHotbarItem(int hotbarItemIndex)
    {
        hotbarItemSlotContainer.GetChild(this.hotbarItemIndex).gameObject.GetComponent<Image>().color = new Color32(173, 173, 173, 255);
        this.hotbarItemIndex = hotbarItemIndex;
        hotbarItemSlotContainer.GetChild(this.hotbarItemIndex).gameObject.GetComponent<Image>().color = new Color32(140, 140, 140, 255);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        if (results.Count > 0)
        {
            int clickedItemIndex = results[0].gameObject.transform.GetSiblingIndex();
            Item clickedItem = inventory.GetItemList()[clickedItemIndex];

            if (holdingItem && clickedItem.itemType == Item.ItemType.Empty)
            {
                inventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
                heldItemImage.sprite = Item.GetSprite(Item.ItemType.Empty);
                holdingItem = false;
            }
            else if (holdingItem && clickedItemIndex == heldItemIndex)
            {
                if (clickedItemIndex < hotbarSize)
                    hotbarItemSlotContainer.GetChild(clickedItemIndex).GetChild(0).gameObject.GetComponent<Image>().sprite = clickedItem.GetSprite();

                itemSlotContainer.GetChild(clickedItemIndex).GetChild(0).GetComponent<Image>().sprite = clickedItem.GetSprite();

                heldItemImage.sprite = Item.GetSprite(Item.ItemType.Empty);

                holdingItem = false;
            }
            else if (!holdingItem && clickedItem.itemType != Item.ItemType.Empty)
            {
                if (clickedItemIndex < hotbarSize)
                    hotbarItemSlotContainer.GetChild(clickedItemIndex).GetChild(0).gameObject.GetComponent<Image>().sprite = Item.GetSprite(Item.ItemType.Empty);

                itemSlotContainer.GetChild(clickedItemIndex).GetChild(0).GetComponent<Image>().sprite = Item.GetSprite(Item.ItemType.Empty);

                heldItemIndex = clickedItemIndex;
                heldItemImage.sprite = Item.GetSprite(clickedItem.itemType);

                holdingItem = true;
            }
        }
    }

    private void Update()
    {
        heldItemRectTransform.anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
    }
}
