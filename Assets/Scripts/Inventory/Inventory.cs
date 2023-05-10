using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventorySize;

    private List<ItemWithAmount> itemList;
    private int firstEmptyIndex;

    public event Action<ItemWithAmount, int> OnItemChangedAtIndex = delegate { };
    public event Action<ItemWithAmount> OnItemAdded = delegate { };
    public event Action<ItemWithAmount> OnItemRemoved = delegate { };

    // Every empty item slot references this instance
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

        InitializeItemListWithSize(inventorySize);
    }

    public void InitializeItemListWithSize(int inventorySize)
    {
        itemList = new List<ItemWithAmount>(inventorySize);

        for (int i = 0; i < inventorySize; ++i)
        {
            itemList.Add(emptyItemInstance);
        }

        firstEmptyIndex = 0;
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

        AddItemAtIndex(newItem, firstEmptyIndex);
    }

    public void AddItemAtIndexWithFallbackToFirstEmptyIndex(ItemWithAmount newItem, int index)
    {
        if (itemList[index].itemData.name == "Empty")
        {
            AddItemAtIndex(newItem, index);
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

        SetFirstEmptyIndex();
    }

    public void SwapItemsBetweenInventories(int index, Inventory otherInventory,
        int otherInventoryIndex)
    {
        ItemWithAmount itemAtIndexBeforeSwap = itemList[index];

        SetItemAtIndex(otherInventory.GetItemList()[otherInventoryIndex], index);
        otherInventory.SetItemAtIndex(itemAtIndexBeforeSwap, otherInventoryIndex);

        SetFirstEmptyIndex();
        otherInventory.SetFirstEmptyIndex();
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

        OnItemRemoved(itemAtIndex);
    }

    private void SetFirstEmptyIndex()
    {
        firstEmptyIndex = itemList.FindIndex(x => x.itemData.name == "Empty");
    }

    private void AddItemAtIndex(ItemWithAmount newItem, int index)
    {
        SetItemAtIndex(newItem, index);

        if (index == firstEmptyIndex)
        {
            SetFirstEmptyIndex();
        }

        OnItemAdded(newItem);
    }

    private void SetItemAtIndex(ItemWithAmount item, int index)
    {
        itemList[index] = item;
        OnItemChangedAtIndex(item, index);
    }

    public ItemWithAmount GetItemAtIndex(int index) => itemList[index];

    public List<ItemWithAmount> GetItemList() => itemList;

    public bool IsFull() => firstEmptyIndex == -1;

    public void SetInventoryFromSerializableInventory(
        SerializableInventory serializableInventory)
    {
        for (int i = 0; i < serializableInventory.SerializableItemList.Count; ++i)
        {
            SerializableInventoryItem serializableInventoryItem =
                serializableInventory.SerializableItemList[i];

            ItemScriptableObject itemScriptableObject =
                Resources.Load<ItemScriptableObject>(
                    $"Items/{serializableInventoryItem.ItemName}");

            ItemWithAmount item = new ItemWithAmount
            {
                itemData = itemScriptableObject,
                amount = serializableInventoryItem.Amount
            };

            SetItemAtIndex(item, i);
        }

        SetFirstEmptyIndex();
    }

    [Serializable]
    public class SerializableInventory
    {
        public List<SerializableInventoryItem> SerializableItemList;
    }

    [Serializable]
    public class SerializableInventoryItem
    {
        public string ItemName;
        public int Amount;
    }

    public void FillInventory()
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

    [ContextMenu("Fill Inventory")]
    private void FillInventoryContextMenuCommand()
    {
        FillInventory();
    }
}
