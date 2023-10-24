using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventorySize;

    private List<ItemStack> itemList;
    private int firstEmptyIndex;

    public event Action<ItemStack, int> OnItemChangedAtIndex = delegate { };
    public event Action<ItemStack> OnItemAdded = delegate { };
    public event Action<ItemStack> OnItemRemoved = delegate { };
    public event Action OnFailedToAddItemToFullInventory = delegate { };

    // Every empty item slot references this instance
    private static ItemStack emptyItemInstance;

    private void Awake()
    {
        if (emptyItemInstance == null)
        {
            ItemScriptableObject emptyItemInfo =
                ItemScriptableObject.FromName("Empty");

            emptyItemInstance = new ItemStack(emptyItemInfo, 0);
        }

        InitializeItemListWithSize(inventorySize);
    }

    public void InitializeItemListWithSize(int inventorySize)
    {
        itemList = new List<ItemStack>(inventorySize);

        for (int i = 0; i < inventorySize; ++i)
        {
            itemList.Add(emptyItemInstance);
        }

        firstEmptyIndex = 0;
    }

    public void AddItem(ItemScriptableObject info, int amount)
    {
        ItemStack newItem = new ItemStack(info, amount);

        AddItem(newItem);
    }

    public void AddItem(ItemStack newItem)
    {
        int itemStackIndex = FindItemStackIndex(newItem);

        if (itemStackIndex != -1)
        {
            AddAmountToItemAtIndex(newItem.amount, itemStackIndex, out int amountAdded);

            int remainingAmount = newItem.amount - amountAdded;

            if (remainingAmount != 0)
            {
                ItemStack itemWithRemainingAmount =
                    newItem.GetCopyWithAmount(remainingAmount);

                AddItem(itemWithRemainingAmount);
            }
        }
        else if (firstEmptyIndex != -1)
        {
            AddItemAtEmptyItemIndex(newItem, firstEmptyIndex);
        }
        else
        {
            Debug.LogWarning("Attempted to add an item to a full inventory");
        }
    }

    public void AddItemToFirstStackOrIndex(ItemStack newItem, int index)
    {
        int itemStackIndex = FindItemStackIndex(newItem);

        if (itemStackIndex != -1)
        {
            AddAmountToItemAtIndex(newItem.amount, itemStackIndex, out int amountAdded);

            int remainingAmount = newItem.amount - amountAdded;

            if (remainingAmount != 0)
            {
                ItemStack itemWithRemainingAmount =
                    newItem.GetCopyWithAmount(remainingAmount);

                AddItemToFirstStackOrIndex(itemWithRemainingAmount, index);
            }
        }
        else if (itemList[index].itemDefinition.IsEmpty())
        {
            AddItemAtEmptyItemIndex(newItem, index);
        }
        else
        {
            AddItem(newItem);
        }
    }

    public void AddAmountToItemAtIndex(int amount, int index, out int amountAdded)
    {
        ItemStack item = itemList[index];

        int stackSize = item.itemDefinition.stackSize;

        amountAdded = Math.Min(amount, stackSize - item.amount);

        item.amount += amountAdded;

        OnItemChangedAtIndex(item, index);

        ItemStack itemAdded = item.GetCopyWithAmount(amountAdded);

        OnItemAdded(itemAdded);
    }

    public void SwapItemsAt(int index1, int index2)
    {
        if (index1 == index2)
        {
            return;
        }

        ItemStack itemAtIndex1BeforeSwap = itemList[index1];

        SetItemAtIndex(itemList[index2], index1);
        SetItemAtIndex(itemAtIndex1BeforeSwap, index2);

        SetFirstEmptyIndex();
    }

    public void SwapItemsBetweenInventories(int index, Inventory otherInventory,
        int otherInventoryIndex)
    {
        ItemStack itemAtIndexBeforeSwap = itemList[index];

        SetItemAtIndex(otherInventory.GetItemList()[otherInventoryIndex], index);
        otherInventory.SetItemAtIndex(itemAtIndexBeforeSwap, otherInventoryIndex);

        SetFirstEmptyIndex();
        otherInventory.SetFirstEmptyIndex();
    }

    public void DecrementItemStackAtIndex(int stackIndex)
    {
        ItemStack itemAtStackIndex = itemList[stackIndex];

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
        ItemStack itemAtStackIndex = itemList[stackIndex];

        itemAtStackIndex.amount += 1;

        OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
    }

    public void RemoveItemAtIndex(int index)
    {
        ItemStack itemAtIndex = itemList[index];

        if (itemAtIndex.itemDefinition.IsEmpty())
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
        firstEmptyIndex = itemList.FindIndex(x => x.itemDefinition.IsEmpty());
    }

    public void AddItemAtIndex(ItemStack newItem, int index)
    {
        ItemStack itemAtIndex = GetItemAtIndex(index);

        if (itemAtIndex.itemDefinition.IsEmpty())
        {
            AddItemAtEmptyItemIndex(newItem, index);
        }
        else if (itemAtIndex.itemDefinition == newItem.itemDefinition)
        {
            AddAmountToItemAtIndex(
                newItem.amount, index, out int amountAdded);

            int remainingAmount = newItem.amount - amountAdded;

            if (remainingAmount != 0)
            {
                ItemStack itemWithRemainingAmount =
                    new ItemStack(newItem.itemDefinition,
                        remainingAmount, newItem.instanceProperties);

                AddItem(itemWithRemainingAmount);
            }
        }
        else
        {
            AddItem(newItem);
        }
    }

    public void AddItemAtEmptyItemIndex(ItemStack newItem, int index)
    {
        // Prevent newItem from being modified
        ItemStack newItemCopy = new ItemStack(newItem);

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

    public void SetItemAtIndex(ItemStack item, int index)
    {
        itemList[index] = item;
        OnItemChangedAtIndex(item, index);
    }

    public void SetItemAmountAtIndex(int amount, int index)
    {
        ItemStack itemAtIndex = itemList[index];

        if (itemAtIndex.itemDefinition.IsEmpty())
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

    public void TryAddItem(ItemStack item, out int amountAdded)
    {
        Action<ItemStack> addItem = (item) => AddItem(item);

        TryAddItem(item, addItem, out amountAdded);
    }

    public void TryAddItemToFirstStackOrIndex(
        ItemStack item, int index, out int amountAdded)
    {
        Action<ItemStack> addItem =
           (item) => AddItemToFirstStackOrIndex(item, index);

        TryAddItem(item, addItem, out amountAdded);
    }

    private void TryAddItem(ItemStack item,
        Action<ItemStack> addItem, out int amountAdded)
    {
        if (!CanAddItem(item, out int canAddAmount))
        {
            if (canAddAmount != 0)
            {
                ItemStack itemToAdd =
                    item.GetCopyWithAmount(canAddAmount);

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
        ItemStack breakableItem = GetItemAtIndex(index);

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

    public bool CanAddItem(ItemStack newItem) => CanAddItem(newItem, out _);

    public bool CanAddItem(ItemStack newItem, out int canAddAmount)
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

                ItemStack currentItem = itemList[i];

                if ((currentItem.itemDefinition.name == newItem.itemDefinition.name &&
                    currentItem.amount < currentItem.itemDefinition.stackSize) ||
                    currentItem.itemDefinition.IsEmpty())
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

            ItemStack itemAtStackIndex = itemList[stackIndex];

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
        ItemStack targetItem = itemList[targetItemIndex];

        if (targetItem.itemDefinition.IsEmpty())
        {
            return;
        }

        for (int i = 0; i < itemList.Count; ++i)
        {
            if (i == targetItemIndex)
            {
                continue;
            }

            ItemStack currentItem = itemList[i];

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

    public void ReplaceItems(List<ItemStack> inputItems, ItemStack outputItem)
    {
        if (!TryRemoveReplaceInputItems(inputItems,
            out Dictionary<int, ItemStack> itemIndexToItemBeforeRemoval))
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

    public void ReplaceItems(List<ItemStack> inputItems,
        List<ItemStack> outputItems)
    {
        if (!TryRemoveReplaceInputItems(inputItems,
            out Dictionary<int, ItemStack> itemIndexToItemBeforeRemoval))
        {
            return;
        }

        Dictionary<int, int> itemIndexToAddAmount = new Dictionary<int, int>();

        foreach (ItemStack outputItem in outputItems)
        {
            if (!CanAddReplaceOutputItem(itemIndexToAddAmount, outputItem))
            {
                RevertItemRemoval(itemIndexToItemBeforeRemoval);
                return;
            }
        }

        foreach(ItemStack outputItem in outputItems)
        {
            AddItem(outputItem);
        }
    }

    private bool TryRemoveReplaceInputItems(List<ItemStack> inputItems,
        out Dictionary<int, ItemStack> itemIndexToItemBeforeRemoval)
    {
        itemIndexToItemBeforeRemoval = null;

        Dictionary<int, int> itemIndexToRemoveAmount = new Dictionary<int, int>();

        foreach (ItemStack inputItem in inputItems)
        {
            if (!HasReplacementInputItem(itemIndexToRemoveAmount, inputItem))
            {
                return false;
            }
        }

        itemIndexToItemBeforeRemoval = new Dictionary<int, ItemStack>();

        foreach (var itemIndexAndRemoveAmount in itemIndexToRemoveAmount)
        {
            int itemToRemoveIndex = itemIndexAndRemoveAmount.Key;
            int itemAmountToRemove = itemIndexAndRemoveAmount.Value;

            ItemStack itemToRemove = itemList[itemToRemoveIndex];

            if (!itemIndexToItemBeforeRemoval.ContainsKey(itemToRemoveIndex))
            {
                itemIndexToItemBeforeRemoval.Add(itemToRemoveIndex,
                    new ItemStack(itemToRemove));
            }

            int newItemAmount = itemToRemove.amount - itemAmountToRemove;
            SetItemAmountAtIndex(newItemAmount, itemToRemoveIndex);
        }

        return true;
    }

    public bool HasReplacementInputItem(
        Dictionary<int, int> itemIndexToExcludeAmount, ItemStack inputItem)
    {
        int amountExcluded = 0;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

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
        Dictionary<int, int> itemIndexToAddAmount, ItemStack outputItem)
    {
        int amountToAdd = outputItem.amount;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if ((currentItem.itemDefinition.name == outputItem.itemDefinition.name &&
                currentItem.amount < currentItem.itemDefinition.stackSize) ||
                currentItem.itemDefinition.IsEmpty())
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
        Dictionary<int, ItemStack> itemIndexToItemBeforeRemoval)
    {
        foreach (var itemIndexAndItemRemoved in itemIndexToItemBeforeRemoval)
        {
            int removedItemIndex = itemIndexAndItemRemoved.Key;
            ItemStack removedItem = itemIndexAndItemRemoved.Value;

            SetItemAtIndex(removedItem, removedItemIndex);
        }
    }

    private int FindItemStackIndex(ItemStack item) =>
        itemList.FindIndex(x => x.itemDefinition.name == item.itemDefinition.name &&
            x.amount < x.itemDefinition.stackSize);

    public ItemStack GetItemAtIndex(int index) => itemList[index];

    public List<ItemStack> GetItemList() => itemList;

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

            ItemStack item = new ItemStack(itemScriptableObject,
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

        public SerializableInventoryItem(ItemStack item)
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

        ItemStack coconutItemWithMaxAmount = new ItemStack(coconutItemInfo,
            coconutItemInfo.stackSize);

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if (currentItem.itemDefinition.name == "Coconut")
            {
                currentItem.amount = coconutItemInfo.stackSize;

                OnItemChangedAtIndex(currentItem, i);
            }
            else if (currentItem.itemDefinition.IsEmpty())
            {
                AddItemAtEmptyItemIndex(coconutItemWithMaxAmount, i);
            }
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            SetItemAtIndex(emptyItemInstance, i);
        }

        firstEmptyIndex = 0;
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
