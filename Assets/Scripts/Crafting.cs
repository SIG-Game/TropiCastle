using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    private Inventory inventory;

    public void CraftItem(List<Item> ingredients, Item resultItem)
    {
        List<Item> itemList = inventory.GetItemList();

        // Add items to remove to a list so that items aren't removed when crafting fails
        List<Item> itemsUsed = new List<Item>();

        // This approach must change when inventory item stacking is added
        foreach (Item ingredient in ingredients) {
            Debug.Log("Finding ingredient " + ingredient.info.name);

            Item ingredientItem = itemList.Find(x => x.info.name == ingredient.info.name && !itemsUsed.Contains(x));

            if (ingredientItem == null)
            {
                Debug.Log("Ingredient " + ingredient.info.name + " not found");
                return;
            }

            Debug.Log("Found ingredient " + ingredient.info.name);

            itemsUsed.Add(ingredientItem);
        }

        foreach (Item item in itemsUsed)
        {
            Debug.Log("Used " + item.info.name);
            inventory.RemoveItem(item);
        }

        inventory.AddItem(resultItem);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
}
