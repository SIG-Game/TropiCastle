using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    private Inventory inventory;

    public void CraftItem(List<Item> ingredients, Item resultItem)
    {
        List<Item> itemList = inventory.GetItemList();

        // Add items to remove to a list so that items aren't removed when crafting fails
        // Slot, amount
        List<(int, int)> itemsUsed = new List<(int, int)>();

        for (int i = 0; i < ingredients.Count; ++i)
        {
            Item ingredient = ingredients[i];

            Debug.Log("Finding ingredient " + ingredient.itemType);

            int usedCount = 0;

            IEnumerable<Item> itemsWithIngredientType = itemList.FindAll(x => x.itemType == ingredient.itemType);

            foreach (Item item in itemsWithIngredientType)
            {
                if (item.amount < ingredient.amount - usedCount)
                {
                    // Current stack is not large enough for current ingredient
                    itemsUsed.Add((itemList.IndexOf(item), item.amount));
                    Debug.Log("Using " + item.amount + " " + item.itemType + " from slot " + itemList.IndexOf(item));
                    
                    usedCount += item.amount;
                }
                else
                {
                    // Current stack is large enough for current ingredient
                    itemsUsed.Add((itemList.IndexOf(item), ingredient.amount - usedCount));
                    Debug.Log("Using " + (ingredient.amount - usedCount) + " " + item.itemType + " from slot " + itemList.IndexOf(item));

                    usedCount = ingredient.amount;
                    break;
                }
            }

            if (usedCount != ingredient.amount)
            {
                Debug.Log("Amount " + ingredient.amount + " of ingredient " + ingredient.itemType + " not found");
                return;
            }
        }

        foreach ((int slot, int amount) in itemsUsed)
        {
            Debug.Log("Removing " + amount + " " + itemList[slot].itemType + " from slot " + slot);
            inventory.RemoveItem(slot, amount);
        }

        inventory.AddItem(resultItem.itemType, resultItem.amount);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
}
