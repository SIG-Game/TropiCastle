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
    public event Action OnFailedToAddItemToFullInventory = delegate { };

    // Every empty item slot references this instance
    private static ItemWithAmount emptyItemInstance;

    private void Awake()
    {
        if (emptyItemInstance == null)
        {
            ItemScriptableObject emptyItemInfo =
                ItemScriptableObject.FromName("Empty");

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
        int remainingAmount = newItem.amount;
        int stackSize = newItem.itemDefinition.stackSize;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemWithAmount currentItem = itemList[i];

            if (currentItem.itemDefinition.name == newItem.itemDefinition.name &&
                currentItem.amount < stackSize)
            {
                AddAmountToItemAtIndex(remainingAmount, i, out int amountAdded);

                remainingAmount -= amountAdded;

                if (remainingAmount == 0)
                {
                    return;
                }
            }
            else if (currentItem.itemDefinition.name == "Empty")
            {
                ItemWithAmount itemWithRemainingAmount = new ItemWithAmount(
                    newItem.itemDefinition, remainingAmount, newItem.instanceProperties);

                AddItemAtIndex(itemWithRemainingAmount, i);

                return;
            }
        }

        Debug.LogWarning("Attempted to add an item to a full inventory");
    }

    public void AddItemAtIndexWithFallbackToFirstEmptyIndex(ItemWithAmount newItem, int index)
    {
        int itemStackIndex = itemList.FindIndex(
            x => x.itemDefinition.name == newItem.itemDefinition.name &&
            x.amount < x.itemDefinition.stackSize);

        if (itemStackIndex != -1)
        {
            AddAmountToItemAtIndex(newItem.amount, itemStackIndex, out int amountAdded);

            int remainingAmount = newItem.amount - amountAdded;

            if (remainingAmount != 0)
            {
                ItemWithAmount itemWithRemainingAmount = new ItemWithAmount(
                    newItem.itemDefinition, remainingAmount, newItem.instanceProperties);

                AddItem(itemWithRemainingAmount);
            }
        }
        else if (itemList[index].itemDefinition.name == "Empty")
        {
            AddItemAtIndex(newItem, index);
        }
        else
        {
            AddItem(newItem);
        }
    }

    public void AddAmountToItemAtIndex(int amount, int index, out int amountAdded)
    {
        ItemWithAmount item = itemList[index];

        int stackSize = item.itemDefinition.stackSize;

        amountAdded = Math.Min(amount, stackSize - item.amount);

        item.amount += amountAdded;

        OnItemChangedAtIndex(item, index);

        ItemWithAmount itemAdded = new ItemWithAmount(
            item.itemDefinition, amountAdded, item.instanceProperties);

        OnItemAdded(itemAdded);
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

        if (itemAtIndex.itemDefinition.name == "Empty")
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
        firstEmptyIndex = itemList.FindIndex(x => x.itemDefinition.name == "Empty");
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

    public void SetItemAmountAtIndex(int amount, int index)
    {
        ItemWithAmount itemAtIndex = itemList[index];

        if (itemAtIndex.itemDefinition.name == "Empty")
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

    public void TryAddItem(ItemWithAmount item, out int amountAdded)
    {
        Action<ItemWithAmount> addItem = (item) => AddItem(item);

        TryAddItem(item, addItem, out amountAdded);
    }

    public void TryAddItemAtIndexWithFallbackToFirstEmptyIndex(
        ItemWithAmount item, int index, out int amountAdded)
    {
        Action<ItemWithAmount> addItem =
           (item) => AddItemAtIndexWithFallbackToFirstEmptyIndex(item, index);

        TryAddItem(item, addItem, out amountAdded);
    }

    private void TryAddItem(ItemWithAmount item,
        Action<ItemWithAmount> addItem, out int amountAdded)
    {
        if (!CanAddItem(item, out int canAddAmount))
        {
            if (canAddAmount != 0)
            {
                ItemWithAmount itemToAdd = new ItemWithAmount(
                    item.itemDefinition, canAddAmount, item.instanceProperties);

                addItem(itemToAdd);
            }
            else
            {
                OnFailedToAddItemToFullInventory();
            }

            amountAdded = canAddAmount;
        }
        else
        {
            addItem(item);

            amountAdded = item.amount;
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

                if ((currentItem.itemDefinition.name == newItem.itemDefinition.name &&
                    currentItem.amount < currentItem.itemDefinition.stackSize) ||
                    currentItem.itemDefinition.name == "Empty")
                {
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
                itemAtStackIndex.itemDefinition.stackSize - itemAtStackIndex.amount;

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

        if (targetItem.itemDefinition.name == "Empty")
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

            if (targetItem.itemDefinition.name == currentItem.itemDefinition.name)
            {
                int combinedAmount = targetItem.amount + currentItem.amount;

                if (combinedAmount <= targetItem.itemDefinition.stackSize)
                {
                    targetItem.amount = combinedAmount;

                    RemoveItemAtIndex(i);
                }
                else
                {
                    targetItem.amount = targetItem.itemDefinition.stackSize;

                    currentItem.amount = combinedAmount - targetItem.itemDefinition.stackSize;

                    OnItemChangedAtIndex(currentItem, i);
                }

                OnItemChangedAtIndex(targetItem, targetItemIndex);
            }
        }
    }

    public void ReplaceItems(List<ItemWithAmount> inputItems, ItemWithAmount outputItem)
    {
        if (!TryRemoveReplaceInputItems(inputItems,
            out Dictionary<int, ItemWithAmount> itemIndexToItemBeforeRemoval))
        {
            return;
        }

        if (CanAddItem(outputItem))
        {
            AddItem(outputItem);
        }
        else
        {
            RevertItemRemoval(itemIndexToItemBeforeRemoval);
        }
    }

    public void ReplaceItems(List<ItemWithAmount> inputItems,
        List<ItemWithAmount> outputItems)
    {
        if (!TryRemoveReplaceInputItems(inputItems,
            out Dictionary<int, ItemWithAmount> itemIndexToItemBeforeRemoval))
        {
            return;
        }

        Dictionary<int, int> itemIndexToAddAmount = new Dictionary<int, int>();

        foreach (ItemWithAmount outputItem in outputItems)
        {
            if (!CanAddReplaceOutputItem(itemIndexToAddAmount, outputItem))
            {
                RevertItemRemoval(itemIndexToItemBeforeRemoval);
                return;
            }
        }

        foreach(ItemWithAmount outputItem in outputItems)
        {
            AddItem(outputItem);
        }
    }

    private bool TryRemoveReplaceInputItems(List<ItemWithAmount> inputItems,
        out Dictionary<int, ItemWithAmount> itemIndexToItemBeforeRemoval)
    {
        itemIndexToItemBeforeRemoval = null;

        Dictionary<int, int> itemIndexToRemoveAmount = new Dictionary<int, int>();

        foreach (ItemWithAmount inputItem in inputItems)
        {
            if (!HasReplacementInputItem(itemIndexToRemoveAmount, inputItem))
            {
                return false;
            }
        }

        itemIndexToItemBeforeRemoval = new Dictionary<int, ItemWithAmount>();

        foreach (var itemIndexAndRemoveAmount in itemIndexToRemoveAmount)
        {
            int itemToRemoveIndex = itemIndexAndRemoveAmount.Key;
            int itemAmountToRemove = itemIndexAndRemoveAmount.Value;

            ItemWithAmount itemToRemove = itemList[itemToRemoveIndex];

            if (!itemIndexToItemBeforeRemoval.ContainsKey(itemToRemoveIndex))
            {
                itemIndexToItemBeforeRemoval.Add(itemToRemoveIndex,
                    new ItemWithAmount(itemToRemove));
            }

            int newItemAmount = itemToRemove.amount - itemAmountToRemove;
            SetItemAmountAtIndex(newItemAmount, itemToRemoveIndex);
        }

        return true;
    }

    public bool HasReplacementInputItem(
        Dictionary<int, int> itemIndexToExcludeAmount, ItemWithAmount inputItem)
    {
        int amountExcluded = 0;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemWithAmount currentItem = itemList[i];

            if (currentItem.itemDefinition.name == inputItem.itemDefinition.name)
            {
                int currentItemAmount = currentItem.amount;

                if (itemIndexToExcludeAmount.ContainsKey(i))
                {
                    currentItemAmount -= itemIndexToExcludeAmount[i];
                }

                if (currentItemAmount + amountExcluded >= inputItem.amount)
                {
                    itemIndexToExcludeAmount.AddOrIncreaseValue(
                        i, inputItem.amount - amountExcluded);

                    return true;
                }
                else
                {
                    itemIndexToExcludeAmount.AddOrIncreaseValue(
                        i, currentItemAmount);

                    amountExcluded += currentItemAmount;
                }
            }
        }

        return false;
    }

    public bool CanAddReplaceOutputItem(
        Dictionary<int, int> itemIndexToAddAmount, ItemWithAmount outputItem)
    {
        int amountToAdd = outputItem.amount;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemWithAmount currentItem = itemList[i];

            if ((currentItem.itemDefinition.name == outputItem.itemDefinition.name &&
                currentItem.amount < currentItem.itemDefinition.stackSize) ||
                currentItem.itemDefinition.name == "Empty")
            {
                int currentItemAmount = currentItem.amount;

                if (itemIndexToAddAmount.ContainsKey(i))
                {
                    currentItemAmount += itemIndexToAddAmount[i];
                }

                if (currentItemAmount + amountToAdd <= outputItem.itemDefinition.stackSize)
                {
                    itemIndexToAddAmount.AddOrIncreaseValue(i, amountToAdd);

                    return true;
                }
                else
                {
                    int amountAdded =
                        outputItem.itemDefinition.stackSize - currentItemAmount;

                    itemIndexToAddAmount.AddOrIncreaseValue(i, amountAdded);

                    amountToAdd -= amountAdded;
                }
            }
        }

        return false;
    }

    private void RevertItemRemoval(
        Dictionary<int, ItemWithAmount> itemIndexToItemBeforeRemoval)
    {
        foreach (var itemIndexAndItemRemoved in itemIndexToItemBeforeRemoval)
        {
            int removedItemIndex = itemIndexAndItemRemoved.Key;
            ItemWithAmount removedItem = itemIndexAndItemRemoved.Value;

            SetItemAtIndex(removedItem, removedItemIndex);
        }
    }

    public ItemWithAmount GetItemAtIndex(int index) => itemList[index];

    public List<ItemWithAmount> GetItemList() => itemList;

    public bool HasNoEmptySlots() => firstEmptyIndex == -1;

    public SerializableInventory GetAsSerializableInventory()
    {
        List<SerializableInventoryItem> serializableItemList =
            itemList.Select(x => new SerializableInventoryItem(x)).ToList();

        var serializableInventory = new SerializableInventory
        {
            SerializableItemList = serializableItemList
        };

        return serializableInventory;
    }

    public void SetUpFromSerializableInventory(SerializableInventory serializableInventory)
    {
        for (int i = 0; i < serializableInventory.SerializableItemList.Count; ++i)
        {
            SerializableInventoryItem serializableInventoryItem =
                serializableInventory.SerializableItemList[i];

            ItemScriptableObject itemScriptableObject =
                ItemScriptableObject.FromName(serializableInventoryItem.ItemName);

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
        public ItemInstanceProperties InstanceProperties;

        public SerializableInventoryItem()
        {
        }

        public SerializableInventoryItem(
            SerializableInventoryItem serializableInventoryItem)
        {
            ItemName = serializableInventoryItem.ItemName;
            Amount = serializableInventoryItem.Amount;
            InstanceProperties = serializableInventoryItem.InstanceProperties?.DeepCopy();
        }

        public SerializableInventoryItem(ItemWithAmount item)
        {
            // Use ScriptableObject name and not item display name
            ItemName = ((ScriptableObject)item.itemDefinition).name;
            Amount = item.amount;
            InstanceProperties = item.instanceProperties;
        }
    }

    public void FillInventory()
    {
        ItemScriptableObject coconutItemInfo =
            ItemScriptableObject.FromName("Coconut");

        ItemWithAmount coconutItemWithMaxAmount = new ItemWithAmount(coconutItemInfo,
            coconutItemInfo.stackSize);

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemWithAmount currentItem = itemList[i];

            if (currentItem.itemDefinition.name == "Coconut")
            {
                currentItem.amount = coconutItemInfo.stackSize;

                OnItemChangedAtIndex(currentItem, i);
            }
            else if (currentItem.itemDefinition.name == "Empty")
            {
                AddItemAtIndex(coconutItemWithMaxAmount, i);
            }
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            if (itemList[i].itemDefinition.name != "Empty")
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
