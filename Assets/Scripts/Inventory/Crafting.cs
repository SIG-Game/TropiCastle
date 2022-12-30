using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    private Inventory inventory;

    public void CraftItem(List<ItemWithAmount> ingredients, ItemWithAmount resultItem)
    {
        List<ItemWithAmount> itemList = inventory.GetItemList();

        // Add items to remove to a list so that items aren't removed when crafting fails
        List<ItemWithAmount> itemsUsed = new List<ItemWithAmount>();

        // This approach must change when inventory item stacking is added
        foreach (ItemWithAmount ingredient in ingredients) {
            Debug.Log("Finding ingredient " + ingredient.itemData.name);

            ItemWithAmount ingredientItem = itemList.Find(x => x.itemData.name == ingredient.itemData.name && !itemsUsed.Contains(x));

            if (ingredientItem == null)
            {
                Debug.Log("Ingredient " + ingredient.itemData.name + " not found");
                return;
            }

            Debug.Log("Found ingredient " + ingredient.itemData.name);

            itemsUsed.Add(ingredientItem);
        }

        foreach (ItemWithAmount item in itemsUsed)
        {
            Debug.Log("Used " + item.itemData.name);
            inventory.RemoveItem(item);
        }

        inventory.AddItem(resultItem);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
}
