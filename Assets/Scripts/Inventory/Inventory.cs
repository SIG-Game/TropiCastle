﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventorySize;

    private List<ItemWithAmount> itemList;
    private int firstEmptyIndex;

    public event Action<ItemWithAmount, int> ChangedItemAtIndex = delegate { };

    // Every empty item slot points to this instance
    private static ItemWithAmount emptyItemInstance;

    private void Awake()
    {
        if (emptyItemInstance == null)
        {
            ItemScriptableObject emptyItemInfo = Resources.Load<ItemScriptableObject>("Items/Empty");

            emptyItemInstance = new ItemWithAmount
            {
                itemData = emptyItemInfo,
                amount = 0
            };
        }

        itemList = new List<ItemWithAmount>(inventorySize);

        for (int i = 0; i < inventorySize; ++i)
        {
            itemList.Add(emptyItemInstance);
        }

        firstEmptyIndex = 0;
    }

    [ContextMenu("Fill Inventory")]
    private void FillInventory()
    {
        ItemScriptableObject coconutItemInfo = Resources.Load<ItemScriptableObject>("Items/Coconut");

        ItemWithAmount coconutItem = new ItemWithAmount
        {
            itemData = coconutItemInfo,
            amount = 1
        };

        while (!IsFull())
        {
            AddItem(coconutItem);
        }
    }

    public void AddItem(ItemScriptableObject info, int amount)
    {
        ItemWithAmount newItem = new ItemWithAmount
        {
            itemData = info,
            amount = amount
        };

        AddItem(newItem);
    }

    public void AddItem(ItemWithAmount newItem)
    {
        if (IsFull())
        {
            Debug.LogWarning("Attempted to add an item to a full inventory");
            return;
        }

        SetItemAtIndex(newItem, firstEmptyIndex);

        firstEmptyIndex = itemList.FindIndex(x => x.itemData.name == "Empty");
    }

    public void AddItemAtIndexWithFallbackToFirstEmptyIndex(ItemWithAmount newItem, int index)
    {
        if (itemList[index].itemData.name == "Empty")
        {
            SetItemAtIndex(newItem, index);
            firstEmptyIndex = itemList.FindIndex(x => x.itemData.name == "Empty");
        }
        else
        {
            AddItem(newItem);
        }
    }

    public void SwapItemsAt(int index1, int index2)
    {
        if (index1 == index2)
        {
            return;
        }

        ItemWithAmount itemAtIndex1BeforeSwap = itemList[index1];

        SetItemAtIndex(itemList[index2], index1);
        SetItemAtIndex(itemAtIndex1BeforeSwap, index2);

        firstEmptyIndex = itemList.FindIndex(x => x.itemData.name == "Empty");
    }

    public void RemoveItemAtIndex(int index)
    {
        ItemWithAmount itemAtIndex = itemList[index];

        if (itemAtIndex.itemData.name == "Empty")
        {
            Debug.LogWarning("Attempted to remove empty item from inventory");
            return;
        }

        SetItemAtIndex(emptyItemInstance, index);

        if (index < firstEmptyIndex || IsFull())
        {
            firstEmptyIndex = index;
        }
    }

    public ItemWithAmount GetItemAtIndex(int index) => itemList[index];

    public List<ItemWithAmount> GetItemList() => itemList;

    public bool IsFull() => firstEmptyIndex == -1;

    private void SetItemAtIndex(ItemWithAmount item, int index)
    {
        itemList[index] = item;
        ChangedItemAtIndex(item, index);
    }
}
