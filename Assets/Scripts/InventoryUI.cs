using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private const int hotbarSize = 10;
    private int hotbarItemIndex = 0;

    public Transform hotbarItemSlotContainer;

    public GameObject player;

    void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
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
}
