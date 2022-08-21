using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public Action<int> ChangedItemAt;

    private List<Item> itemList;
    private Action<Item> useItemAction;

    // Every empty item slot points to this instance
    private Item emptyItemInstance;

    private int firstEmptyIndex;

    private const int inventorySize = 15;

    public Inventory(Action<Item> useItemAction, ItemScriptableObject emptyItemInfo)
    {
        this.useItemAction = useItemAction;

        emptyItemInstance = new Item
        {
            amount = 0,
            info = emptyItemInfo
        };

        itemList = new List<Item>(inventorySize);

        for (int i = 0; i < inventorySize; ++i)
        {
            itemList.Add(emptyItemInstance);
        }

        firstEmptyIndex = 0;
    }

    public void AddItem(ItemScriptableObject info, int amount)
    {
        Item newItem = new Item
        {
            info = info,
            amount = amount
        };

        AddItem(newItem);
    }

    public void AddItem(Item newItem)
    {
        if (IsFull())
            return;

        itemList[firstEmptyIndex] = newItem;
        ChangedItemAt?.Invoke(firstEmptyIndex);
        firstEmptyIndex = itemList.FindIndex(x => x.info.name == "Empty");
    }

    public void SwapItemsAt(int index1, int index2)
    {
        Item temp = itemList[index1];
        itemList[index1] = itemList[index2];
        itemList[index2] = temp;

        ChangedItemAt?.Invoke(index1);
        ChangedItemAt?.Invoke(index2);

        firstEmptyIndex = itemList.FindIndex(x => x.info.name == "Empty");
    }

    public void RemoveItem(Item item)
    {
        if (item.info.name == "Empty")
            return;

        int itemIndex = itemList.IndexOf(item);
        if (itemIndex < firstEmptyIndex || IsFull())
            firstEmptyIndex = itemIndex;

        itemList[itemIndex] = emptyItemInstance;

        ChangedItemAt?.Invoke(itemIndex);
    }

    public void UseItem(Item item)
    {
        useItemAction(item);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public bool IsFull()
    {
        return firstEmptyIndex == -1;
    }
}
