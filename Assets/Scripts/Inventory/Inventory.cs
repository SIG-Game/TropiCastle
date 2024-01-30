using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventorySize;

    [JsonProperty] private List<ItemStack> itemList;

    private int firstEmptyIndex;

    public event Action<ItemStack, int> OnItemChangedAtIndex = (_, _) => {};
    public event Action OnFailedToAddItemToFullInventory = () => {};

    private void Awake()
    {
        InitializeItemListWithSize(inventorySize);
    }

    public void InitializeItemListWithSize(int inventorySize)
    {
        itemList = new List<ItemStack>(inventorySize);

        var emptyItem = new ItemStack("Empty", 0);

        for (int i = 0; i < inventorySize; ++i)
        {
            itemList.Add(emptyItem);
        }

        firstEmptyIndex = 0;
    }

    public void AddItem(ItemScriptableObject info, int amount)
    {
        ItemStack newItem = new(info, amount);

        AddItem(newItem);
    }

    public void AddItem(ItemStack newItem)
    {
        int itemStackIndex = FindItemStackIndex(newItem);

        if (itemStackIndex != -1)
        {
            AddAmountToItemAtIndex(newItem.Amount, itemStackIndex, out int amountAdded);

            int remainingAmount = newItem.Amount - amountAdded;

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
            AddAmountToItemAtIndex(newItem.Amount, itemStackIndex, out int amountAdded);

            int remainingAmount = newItem.Amount - amountAdded;

            if (remainingAmount != 0)
            {
                ItemStack itemWithRemainingAmount =
                    newItem.GetCopyWithAmount(remainingAmount);

                AddItemToFirstStackOrIndex(itemWithRemainingAmount, index);
            }
        }
        else if (itemList[index].ItemDefinition.IsEmpty())
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

        int stackSize = item.ItemDefinition.StackSize;

        amountAdded = Math.Min(amount, stackSize - item.Amount);

        item.Amount += amountAdded;

        itemList[index] = item;

        OnItemChangedAtIndex(item, index);
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

        SetItemAtIndex(otherInventory.itemList[otherInventoryIndex], index);
        otherInventory.SetItemAtIndex(itemAtIndexBeforeSwap, otherInventoryIndex);

        SetFirstEmptyIndex();
        otherInventory.SetFirstEmptyIndex();
    }

    public void DecrementItemStackAtIndex(int stackIndex)
    {
        ItemStack itemAtStackIndex = itemList[stackIndex];

        if (itemAtStackIndex.Amount == 1)
        {
            RemoveItemAtIndex(stackIndex);
        }
        else
        {
            itemAtStackIndex.Amount -= 1;

            itemList[stackIndex] = itemAtStackIndex;

            OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
        }
    }

    public void IncrementItemStackAtIndex(int stackIndex)
    {
        ItemStack itemAtStackIndex = itemList[stackIndex];

        itemAtStackIndex.Amount += 1;

        itemList[stackIndex] = itemAtStackIndex;

        OnItemChangedAtIndex(itemAtStackIndex, stackIndex);
    }

    public void RemoveItem(ItemStack itemToRemove)
    {
        int amountToRemove = itemToRemove.Amount;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if (currentItem.ItemDefinition.name == itemToRemove.ItemDefinition.name)
            {
                int currentItemAmountToRemove =
                    Math.Min(currentItem.Amount, amountToRemove);

                SetItemAmountAtIndex(
                    currentItem.Amount - currentItemAmountToRemove, i);

                amountToRemove -= currentItemAmountToRemove;

                if (amountToRemove == 0)
                {
                    return;
                }
            }
        }

        Debug.LogError($"Failed to remove item from inventory: {itemToRemove}");
    }

    public void RemoveItemAtIndex(int index)
    {
        ItemStack itemAtIndex = itemList[index];

        if (itemAtIndex.ItemDefinition.IsEmpty())
        {
            Debug.LogWarning("Attempted to remove empty item from inventory");
            return;
        }

        SetItemAtIndex(new ItemStack("Empty", 0), index);

        if (index < firstEmptyIndex || HasNoEmptySlots())
        {
            firstEmptyIndex = index;
        }
    }

    private void SetFirstEmptyIndex()
    {
        firstEmptyIndex = itemList.FindIndex(x => x.ItemDefinition.IsEmpty());
    }

    public void AddItemAtIndex(ItemStack newItem, int index)
    {
        ItemStack itemAtIndex = GetItemAtIndex(index);

        if (itemAtIndex.ItemDefinition.IsEmpty())
        {
            AddItemAtEmptyItemIndex(newItem, index);
        }
        else if (itemAtIndex.ItemDefinition == newItem.ItemDefinition)
        {
            AddAmountToItemAtIndex(
                newItem.Amount, index, out int amountAdded);

            int remainingAmount = newItem.Amount - amountAdded;

            if (remainingAmount != 0)
            {
                ItemStack itemWithRemainingAmount =
                    new(newItem.ItemDefinition, remainingAmount,
                        newItem.InstanceProperties);

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
        ItemStack newItemCopy = new(newItem);

        if (newItemCopy.InstanceProperties == null ||
            newItemCopy.InstanceProperties.PropertyDictionary.Count == 0)
        {
            newItemCopy.InitializeItemInstanceProperties();
        }

        SetItemAtIndex(newItemCopy, index);

        if (index == firstEmptyIndex)
        {
            SetFirstEmptyIndex();
        }
    }

    public void SetItemAtIndex(ItemStack item, int index)
    {
        itemList[index] = item;
        OnItemChangedAtIndex(item, index);
    }

    public void SetItemAmountAtIndex(int amount, int index)
    {
        ItemStack itemAtIndex = itemList[index];

        if (itemAtIndex.ItemDefinition.IsEmpty())
        {
            return;
        }

        if (amount != 0)
        {
            itemAtIndex.Amount = amount;

            itemList[index] = itemAtIndex;

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

            amountAdded = item.Amount;
        }
    }

    public void DecrementItemDurabilityAtIndex(int index)
    {
        ItemStack breakableItem = GetItemAtIndex(index);

        if (breakableItem.InstanceProperties.HasProperty("Durability"))
        {
            int durability = breakableItem.InstanceProperties
                .GetIntProperty("Durability") - 1;

            if (durability == 0)
            {
                RemoveItemAtIndex(index);
            }
            else
            {
                breakableItem.InstanceProperties
                    .SetProperty("Durability", durability);

                OnItemChangedAtIndex(breakableItem, index);
            }
        }
    }

    public bool CanAddItem(ItemStack itemToAdd) => CanAddItem(itemToAdd, out _);

    public bool CanAddItem(ItemStack itemToAdd, out int canAddAmount)
    {
        if (firstEmptyIndex != -1)
        {
            canAddAmount = itemToAdd.Amount;

            return true;
        }

        int amountToAdd = itemToAdd.Amount;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if (currentItem.ItemDefinition.name == itemToAdd.ItemDefinition.name)
            {
                amountToAdd -= Math.Min(amountToAdd,
                    currentItem.ItemDefinition.StackSize - currentItem.Amount);

                if (amountToAdd == 0)
                {
                    canAddAmount = itemToAdd.Amount;

                    return true;
                }
            }
        }

        canAddAmount = itemToAdd.Amount - amountToAdd;

        return false;
    }

    public bool CanRemoveItem(ItemStack itemToRemove)
    {
        int amountToRemove = itemToRemove.Amount;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if (currentItem.ItemDefinition.name == itemToRemove.ItemDefinition.name)
            {
                amountToRemove -= Math.Min(amountToRemove, currentItem.Amount);

                if (amountToRemove == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ConsolidateItemsToIndex(int targetItemIndex)
    {
        ItemStack targetItem = itemList[targetItemIndex];

        if (targetItem.ItemDefinition.IsEmpty())
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

            if (targetItem.ItemDefinition.name == currentItem.ItemDefinition.name)
            {
                int combinedAmount = targetItem.Amount + currentItem.Amount;

                if (combinedAmount <= targetItem.ItemDefinition.StackSize)
                {
                    targetItem.Amount = combinedAmount;

                    itemList[targetItemIndex] = targetItem;

                    RemoveItemAtIndex(i);
                }
                else
                {
                    targetItem.Amount = targetItem.ItemDefinition.StackSize;

                    itemList[targetItemIndex] = targetItem;

                    currentItem.Amount = combinedAmount - targetItem.ItemDefinition.StackSize;

                    itemList[i] = currentItem;

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

        if (!CanAddReplaceOutputItems(outputItems))
        {
            RevertItemRemoval(itemIndexToItemBeforeRemoval);
            return;
        }

        foreach (ItemStack outputItem in outputItems)
        {
            AddItem(outputItem);
        }
    }

    public bool CanReplaceItems(
        List<ItemStack> inputItems, List<ItemStack> outputItems)
    {
        if (!TryRemoveReplaceInputItems(inputItems,
            out Dictionary<int, ItemStack> itemIndexToItemBeforeRemoval))
        {
            return false;
        }

        bool canAddReplaceOutputItems = CanAddReplaceOutputItems(outputItems);

        RevertItemRemoval(itemIndexToItemBeforeRemoval);

        return canAddReplaceOutputItems;
    }

    private bool TryRemoveReplaceInputItems(List<ItemStack> inputItems,
        out Dictionary<int, ItemStack> itemIndexToItemBeforeRemoval)
    {
        itemIndexToItemBeforeRemoval = null;

        Dictionary<int, int> itemIndexToRemoveAmount = new();

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

            int newItemAmount = itemToRemove.Amount - itemAmountToRemove;
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

            if (currentItem.ItemDefinition.name == inputItem.ItemDefinition.name)
            {
                int currentItemAmount = currentItem.Amount;

                if (itemIndexToExcludeAmount.ContainsKey(i))
                {
                    currentItemAmount -= itemIndexToExcludeAmount[i];
                }

                if (currentItemAmount + amountExcluded >= inputItem.Amount)
                {
                    itemIndexToExcludeAmount.AddOrIncreaseValue(
                        i, inputItem.Amount - amountExcluded);

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

    private bool CanAddReplaceOutputItems(List<ItemStack> outputItems)
    {
        Dictionary<int, int> itemIndexToAddAmount = new();

        foreach (ItemStack outputItem in outputItems)
        {
            if (!CanAddReplaceOutputItem(itemIndexToAddAmount, outputItem))
            {
                return false;
            }
        }

        return true;
    }

    private bool CanAddReplaceOutputItem(
        Dictionary<int, int> itemIndexToAddAmount, ItemStack outputItem)
    {
        int amountToAdd = outputItem.Amount;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if ((currentItem.ItemDefinition.name == outputItem.ItemDefinition.name &&
                currentItem.Amount < currentItem.ItemDefinition.StackSize) ||
                currentItem.ItemDefinition.IsEmpty())
            {
                int currentItemAmount = currentItem.Amount;

                if (itemIndexToAddAmount.ContainsKey(i))
                {
                    currentItemAmount += itemIndexToAddAmount[i];
                }

                if (currentItemAmount + amountToAdd <= outputItem.ItemDefinition.StackSize)
                {
                    itemIndexToAddAmount.AddOrIncreaseValue(i, amountToAdd);

                    return true;
                }
                else
                {
                    int amountAdded =
                        outputItem.ItemDefinition.StackSize - currentItemAmount;

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
        itemList.FindIndex(x => x.ItemDefinition.name == item.ItemDefinition.name &&
            x.Amount < x.ItemDefinition.StackSize);

    public ItemStack GetItemAtIndex(int index) => itemList[index];

    public List<ItemStack> GetItemList() => itemList;

    public bool HasNoEmptySlots() => firstEmptyIndex == -1;

    public void SetUpFromItemList(List<ItemStack> itemList)
    {
        for (int i = 0; i < itemList.Count; ++i)
        {
            SetItemAtIndex(itemList[i], i);
        }

        SetFirstEmptyIndex();
    }

    public void FillInventory()
    {
        ItemScriptableObject coconutItemDefinition =
            ItemScriptableObject.FromName("Coconut");

        ItemStack coconutItemWithMaxAmount = new(
            coconutItemDefinition, coconutItemDefinition.StackSize);

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemStack currentItem = itemList[i];

            if (currentItem.ItemDefinition.name == "Coconut")
            {
                currentItem.Amount = coconutItemDefinition.StackSize;

                OnItemChangedAtIndex(currentItem, i);
            }
            else if (currentItem.ItemDefinition.IsEmpty())
            {
                AddItemAtEmptyItemIndex(coconutItemWithMaxAmount, i);
            }
        }
    }

    public void ClearInventory()
    {
        var emptyItem = new ItemStack("Empty", 0);

        for (int i = 0; i < itemList.Count; ++i)
        {
            SetItemAtIndex(emptyItem, i);
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
