using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private const int numHotbarItems = 10;

    public Transform hotbarItemSlotContainer;

    public GameObject player;

    void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems()
    {
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Transform child in hotbarItemSlotContainer)
        {
            child.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 55f;

        List<Item> inventoryItemList = inventory.GetItemList();

        for (int i = 0; i < numHotbarItems && i < inventoryItemList.Count; ++i)
        {
            Item item = inventoryItemList[i];
            hotbarItemSlotContainer.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().sprite = item.GetSprite();
        }

        foreach (Item item in inventoryItemList)
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            itemSlotRectTransform.GetComponent<UIButton>().onLeftClick.AddListener(() => {
                inventory.UseItem(item);
            });

            itemSlotRectTransform.GetComponent<UIButton>().onRightClick.AddListener(() => {
                inventory.RemoveItem(item);
                ItemWorld.DropItem(player.transform.position, item);
            });

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);

            Image image = itemSlotRectTransform.GetChild(0).GetComponent<Image>();
            image.sprite = item.GetSprite();

            ++x;
            if (x > 4)
            {
                x = 0;
                --y;
            }
        }
    }
}
