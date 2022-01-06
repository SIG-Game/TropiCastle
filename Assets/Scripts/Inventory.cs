using System;
using System.Collections.Generic;

public class Inventory
{
    public Action<int> ChangedItemAt;

    private List<Item> itemList;
    private Action<Item> useItemAction;

    private int firstEmptyIndex;

    private const int inventorySize = 15;

    public Inventory(Action<Item> useItemAction)
    {
        this.useItemAction = useItemAction;

        itemList = new List<Item>(inventorySize);

        for (int i = 0; i < inventorySize; ++i)
        {
            itemList.Add(new Item());
        }

        firstEmptyIndex = 0;
    }

    public void AddItem(Item.ItemType itemType, int amount)
    {
        if (IsFull())
            return;

        itemList[firstEmptyIndex].itemType = itemType;
        itemList[firstEmptyIndex].amount = amount;
        ChangedItemAt?.Invoke(firstEmptyIndex);
        firstEmptyIndex = itemList.FindIndex(x => x.itemType == Item.ItemType.Empty);
    }

    public void SwapItemsAt(int index1, int index2)
    {
        Item temp = itemList[index1];
        itemList[index1] = itemList[index2];
        itemList[index2] = temp;

        ChangedItemAt?.Invoke(index1);
        ChangedItemAt?.Invoke(index2);

        firstEmptyIndex = itemList.FindIndex(x => x.itemType == Item.ItemType.Empty);
    }

    public void RemoveItem(Item item)
    {
        if (item.itemType == Item.ItemType.Empty)
            return;

        item.itemType = Item.ItemType.Empty;

        int itemIndex = itemList.IndexOf(item);
        if (itemIndex < firstEmptyIndex || IsFull())
            firstEmptyIndex = itemIndex;

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
