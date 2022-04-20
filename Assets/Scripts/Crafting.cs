using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    private Inventory inventory;

    public void CraftItem(List<Item.ItemType> ingredients, Item.ItemType resultItemType, int resultAmount)
    {
        List<Item> itemList = inventory.GetItemList();

        // Add items to remove to a list so that items aren't removed when crafting fails
        List<Item> itemsUsed = new List<Item>();

        // This approach must change when inventory item stacking is added
        foreach (Item.ItemType ingredient in ingredients) {
            Debug.Log("Finding ingredient " + ingredient);

            Item ingredientItem = itemList.Find(x => x.itemType == ingredient && !itemsUsed.Contains(x));

            if (ingredientItem == null)
            {
                Debug.Log("Ingredient " + ingredient + " not found");
                return;
            }

            Debug.Log("Found ingredient " + ingredient);

            itemsUsed.Add(ingredientItem);
        }

        foreach (Item item in itemsUsed)
        {
            Debug.Log("Used " + item.itemType);
            inventory.RemoveItem(item);
        }

        inventory.AddItem(resultItemType, resultAmount);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
}
