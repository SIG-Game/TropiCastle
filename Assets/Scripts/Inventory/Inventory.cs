using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour, ISavable<Inventory.SerializableInventory>
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

            emptyItemInstance = new ItemWithAmount(emptyItemInfo, 0);
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
        ItemWithAmount newItem = new ItemWithAmount(info, amount);

        AddItem(newItem);
    }

    public void AddItem(ItemWithAmount newItem)
    {
        int stackIndex = FindStackIndex(newItem);

        if (HasNoEmptySlots() && stackIndex == -1)
        {
            Debug.LogWarning("Attempted to add an item to a full inventory");
            return;
        }

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
        }
        else
        {
            itemAtStackIndex.amount -= 1;

            OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
        }
    }

    public void IncrementItemStackAtIndex(int stackIndex)
    {
        ItemWithAmount itemAtStackIndex = itemList[stackIndex];

        itemAtStackIndex.amount += 1;

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

        if (index < firstEmptyIndex || HasNoEmptySlots())
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

        int itemStackSize = newItem.itemData.stackSize;

        int combinedAmount = itemAtStackIndex.amount + newItem.amount;

        if (combinedAmount <= itemStackSize)
        {
            itemAtStackIndex.amount = combinedAmount;

            OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
            OnItemAdded(newItem);
        }
        else
        {
            int amountToAdd = itemStackSize - itemAtStackIndex.amount;

            itemAtStackIndex.amount = itemStackSize;

            OnItemChangedAtIndex(itemAtStackIndex, stackIndex);

            ItemWithAmount itemWithAmountAdded = new ItemWithAmount(newItem.itemData,
                amountToAdd, newItem.instanceProperties);

            OnItemAdded(itemWithAmountAdded);

            int amountRemaining = combinedAmount - itemStackSize;

            ItemWithAmount itemWithAmountRemaining = new ItemWithAmount(newItem.itemData,
                amountRemaining, newItem.instanceProperties);

            AddItem(itemWithAmountRemaining);
        }
    }

    public void AddItemAtIndex(ItemWithAmount newItem, int index)
    {
        // Prevent newItem from being modified
        ItemWithAmount newItemCopy = new ItemWithAmount(newItem);

        if (newItemCopy.instanceProperties == null)
        {
            newItemCopy.InitializeItemInstanceProperties();
        }

        SetItemAtIndex(newItemCopy, index);

        if (index == firstEmptyIndex)
        {
            SetFirstEmptyIndex();
        }

        OnItemAdded(newItemCopy);
    }

    public void SetItemAtIndex(ItemWithAmount item, int index)
    {
        itemList[index] = item;
        OnItemChangedAtIndex(item, index);
    }

    private int FindStackIndex(ItemWithAmount item) =>
        itemList.FindIndex(x => x.itemData.name == item.itemData.name &&
            x.amount < x.itemData.stackSize);

    public void SetItemAmountAtIndex(int amount, int index)
    {
        ItemWithAmount itemAtIndex = itemList[index];

        if (itemAtIndex.itemData.name == "Empty")
        {
            return;
        }

        if (amount != 0)
        {
            itemAtIndex.amount = amount;

            OnItemChangedAtIndex(itemAtIndex, index);
        }
        else
        {
            RemoveItemAtIndex(index);
        }
    }

    public void DecrementItemDurabilityAtIndex(int index)
    {
        ItemWithAmount breakableItem = GetItemAtIndex(index);

        if (breakableItem.instanceProperties is
            BreakableItemInstanceProperties breakableItemInstanceProperties)
        {
            breakableItemInstanceProperties.Durability--;

            if (breakableItemInstanceProperties.Durability == 0)
            {
                RemoveItemAtIndex(index);
            }
            else
            {
                OnItemChangedAtIndex(breakableItem, index);
            }
        }
    }

    public bool CanAddItem(ItemWithAmount newItem) => CanAddItem(newItem, out _);

    public bool CanAddItem(ItemWithAmount newItem, out int canAddAmount)
    {
        if (firstEmptyIndex != -1)
        {
            canAddAmount = newItem.amount;
            return true;
        }

        HashSet<int> itemSlotsFilled = new HashSet<int>();

        int amountToAdd = newItem.amount;

        while (amountToAdd > 0)
        {
            int stackIndex = -1;

            for (int i = 0; i < itemList.Count; ++i)
            {
                if (itemSlotsFilled.Contains(i))
                {
                    continue;
                }

                ItemWithAmount currentItem = itemList[i];

                if ((currentItem.itemData.name == newItem.itemData.name &&
                    currentItem.amount < currentItem.itemData.stackSize) ||
                    currentItem.itemData.name == "Empty") {
                    stackIndex = i;

                    break;
                }
            }

            if (stackIndex == -1)
            {
                canAddAmount = newItem.amount - amountToAdd;
                return false;
            }

            ItemWithAmount itemAtStackIndex = itemList[stackIndex];

            int amountToReachStackSizeLimit =
                itemAtStackIndex.itemData.stackSize - itemAtStackIndex.amount;

            if (amountToReachStackSizeLimit < amountToAdd)
            {
                itemSlotsFilled.Add(stackIndex);

                amountToAdd -= amountToReachStackSizeLimit;
            }
            else
            {
                amountToAdd = 0;
            }
        }

        canAddAmount = newItem.amount;
        return true;
    }

    public void ConsolidateItemsToIndex(int targetItemIndex)
    {
        ItemWithAmount targetItem = itemList[targetItemIndex];

        if (targetItem.itemData.name == "Empty")
        {
            return;
        }

        for (int i = 0; i < itemList.Count; ++i)
        {
            if (i == targetItemIndex)
            {
                continue;
            }

            ItemWithAmount currentItem = itemList[i];

            if (targetItem.itemData.name == currentItem.itemData.name)
            {
                int combinedAmount = targetItem.amount + currentItem.amount;

                if (combinedAmount <= targetItem.itemData.stackSize)
                {
                    targetItem.amount = combinedAmount;

                    RemoveItemAtIndex(i);
                }
                else
                {
                    targetItem.amount = targetItem.itemData.stackSize;

                    currentItem.amount = combinedAmount - targetItem.itemData.stackSize;

                    OnItemChangedAtIndex(currentItem, i);
                }

                OnItemChangedAtIndex(targetItem, targetItemIndex);
            }
        }
    }

    public ItemWithAmount GetItemAtIndex(int index) => itemList[index];

    public List<ItemWithAmount> GetItemList() => itemList;

    public bool HasNoEmptySlots() => firstEmptyIndex == -1;

    public SerializableInventory GetSerializableState()
    {
        IEnumerable<SerializableInventoryItem> serializableInventoryItems =
            itemList.Select(x => new SerializableInventoryItem(x));

        var serializableState = new SerializableInventory
        {
            SerializableItemList = serializableInventoryItems.ToList()
        };

        return serializableState;
    }

    public void SetPropertiesFromSerializableState(
        SerializableInventory serializableState)
    {
        for (int i = 0; i < serializableState.SerializableItemList.Count; ++i)
        {
            SerializableInventoryItem serializableInventoryItem =
                serializableState.SerializableItemList[i];

            ItemScriptableObject itemScriptableObject =
                Resources.Load<ItemScriptableObject>(
                    $"Items/{serializableInventoryItem.ItemName}");

            ItemWithAmount item = new ItemWithAmount(itemScriptableObject,
                serializableInventoryItem.Amount,
                serializableInventoryItem.InstanceProperties);

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

        public SerializableInventoryItem()
        {
        }

        public SerializableInventoryItem(ItemWithAmount item)
        {
            // Use ScriptableObject name and not item display name
            ItemName = ((ScriptableObject)item.itemData).name;
            Amount = item.amount;
            InstanceProperties = item.instanceProperties;
        }
    }

    public void FillInventory()
    {
        ItemScriptableObject coconutItemInfo = Resources.Load<ItemScriptableObject>("Items/Coconut");

        ItemWithAmount coconutItemWithMaxAmount = new ItemWithAmount(coconutItemInfo,
            coconutItemInfo.stackSize);

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemWithAmount currentItem = itemList[i];

            if (currentItem.itemData.name == "Coconut")
            {
                currentItem.amount = coconutItemInfo.stackSize;

                OnItemChangedAtIndex(currentItem, i);
            }
            else if (currentItem.itemData.name == "Empty")
            {
                AddItemAtIndex(coconutItemWithMaxAmount, i);
            }
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            if (itemList[i].itemData.name != "Empty")
            {
                RemoveItemAtIndex(i);
            }
        }
    }

    [ContextMenu("Fill Inventory")]
    private void FillInventoryContextMenuCommand()
    {
        FillInventory();
    }

    [ContextMenu("Clear Inventory")]
    private void ClearInventoryContextMenuCommand()
    {
        ClearInventory();
    }
}
