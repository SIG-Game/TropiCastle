using System;
using System.Collections.Generic;
using System.Linq;
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

        newItem.SetItemInstanceProperties();

        AddItem(newItem);
    }

    public void AddItem(ItemWithAmount newItem)
    {
        if (IsFull())
        {
            Debug.LogWarning("Attempted to add an item to a full inventory");
            return;
        }

        int stackIndex = FindStackIndex(newItem);

        if (stackIndex != -1)
        {
            AddItemToStackAtIndex(newItem, stackIndex);
        }
        else
        {
            AddItemAtIndex(newItem, firstEmptyIndex);
        }
    }

    public void AddItemAtIndexWithFallbackToFirstEmptyIndex(ItemWithAmount newItem, int index)
    {
        int stackIndex = FindStackIndex(newItem);

        if (stackIndex != -1)
        {
            AddItemToStackAtIndex(newItem, stackIndex);
        }
        else if (itemList[index].itemData.name == "Empty")
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

    public void DecrementItemStackAtIndex(int stackIndex)
    {
        ItemWithAmount itemAtStackIndex = itemList[stackIndex];

        if (itemAtStackIndex.amount == 1)
        {
            RemoveItemAtIndex(stackIndex);

            OnItemRemoved(itemAtStackIndex);
        }
        else
        {
            itemAtStackIndex.amount -= 1;
        }

        OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
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

    private void AddItemToStackAtIndex(ItemWithAmount newItem, int stackIndex)
    {
        ItemWithAmount itemAtStackIndex = itemList[stackIndex];

        itemAtStackIndex.amount += newItem.amount;

        OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
        OnItemAdded(newItem);
    }

    private void AddItemAtIndex(ItemWithAmount newItem, int index)
    {
        // Prevent newItem from being modified
        ItemWithAmount newItemCopy = new ItemWithAmount
        {
            itemData = newItem.itemData,
            amount = newItem.amount,
            instanceProperties = newItem.instanceProperties
        };

        SetItemAtIndex(newItemCopy, index);

        if (index == firstEmptyIndex)
        {
            SetFirstEmptyIndex();
        }

        OnItemAdded(newItemCopy);
    }

    private void SetItemAtIndex(ItemWithAmount item, int index)
    {
        itemList[index] = item;
        OnItemChangedAtIndex(item, index);
    }

    public void InvokeOnItemChangedAtIndexEvent(ItemWithAmount item, int index)
    {
        OnItemChangedAtIndex(item, index);
    }

    private int FindStackIndex(ItemWithAmount item) =>
        itemList.FindIndex(x => x.itemData.name == item.itemData.name &&
            x.amount < x.itemData.stackSize);

    public ItemWithAmount GetItemAtIndex(int index) => itemList[index];

    public List<ItemWithAmount> GetItemList() => itemList;

    public bool IsFull() => firstEmptyIndex == -1;

    public SerializableInventory GetSerializableInventory()
    {
        IEnumerable<SerializableInventoryItem> serializableInventoryItems =
            itemList.Select(x => new SerializableInventoryItem
            {
                // Use ScriptableObject name and not item display name
                ItemName = ((ScriptableObject)x.itemData).name,
                Amount = x.amount,
                InstanceProperties = x.instanceProperties
            });

        var serializableInventory = new SerializableInventory
        {
            SerializableItemList = serializableInventoryItems.ToList()
        };

        return serializableInventory;
    }

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
                amount = serializableInventoryItem.Amount,
                instanceProperties = serializableInventoryItem.InstanceProperties
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

        [SerializeReference]
        public object InstanceProperties;
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
