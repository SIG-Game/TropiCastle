using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public Action<int> ChangedItemAt;

    private List<ItemWithAmount> itemList;
    private Action<ItemWithAmount> useItemAction;

    // Every empty item slot points to this instance
    private ItemWithAmount emptyItemInstance;

    private int firstEmptyIndex;

    private const int inventorySize = 15;

    public Inventory(Action<ItemWithAmount> useItemAction, ItemScriptableObject emptyItemInfo)
    {
        this.useItemAction = useItemAction;

        emptyItemInstance = new ItemWithAmount
        {
            amount = 0,
            itemData = emptyItemInfo
        };

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
            return;

        itemList[firstEmptyIndex] = newItem;
        ChangedItemAt?.Invoke(firstEmptyIndex);
        firstEmptyIndex = itemList.FindIndex(x => x.itemData.name == "Empty");
    }

    public void SwapItemsAt(int index1, int index2)
    {
        ItemWithAmount temp = itemList[index1];
        itemList[index1] = itemList[index2];
        itemList[index2] = temp;

        ChangedItemAt?.Invoke(index1);
        ChangedItemAt?.Invoke(index2);

        firstEmptyIndex = itemList.FindIndex(x => x.itemData.name == "Empty");
    }

    public void RemoveItem(ItemWithAmount item)
    {
        if (item.itemData.name == "Empty")
            return;

        int itemIndex = itemList.IndexOf(item);
        if (itemIndex < firstEmptyIndex || IsFull())
            firstEmptyIndex = itemIndex;

        itemList[itemIndex] = emptyItemInstance;

        ChangedItemAt?.Invoke(itemIndex);
    }

    public void UseItem(ItemWithAmount item)
    {
        useItemAction(item);
    }

    public List<ItemWithAmount> GetItemList()
    {
        return itemList;
    }

    public bool IsFull()
    {
        return firstEmptyIndex == -1;
    }
}
