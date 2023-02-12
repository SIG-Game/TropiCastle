﻿using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private Inventory inventory;

    private void Start()
    {
        inventory = player.GetInventory();
    }

    public void CraftItem(CraftingRecipeScriptableObject craftingRecipe)
    {
        List<ItemWithAmount> itemList = inventory.GetItemList();

        // Add indexes of items to remove to a list so that items aren't removed when crafting fails
        List<int> itemsToRemoveIndexes = new List<int>();

        // This approach must change when inventory item stacking is added
        foreach (ItemWithAmount ingredient in craftingRecipe.ingredients)
        {
            ItemWithAmount inventoryIngredientItem = null;

            for (int i = 0; i < itemList.Count; ++i)
            {
                ItemWithAmount currentItem = itemList[i];

                if (currentItem.itemData.name == ingredient.itemData.name && !itemsToRemoveIndexes.Contains(i))
                {
                    inventoryIngredientItem = currentItem;
                    itemsToRemoveIndexes.Add(i);
                    Debug.Log("Found ingredient " + ingredient.itemData.name);
                    break;
                }
            }

            if (inventoryIngredientItem == null)
            {
                Debug.Log("Ingredient " + ingredient.itemData.name + " not found");
                return;
            }
        }

        foreach (int itemIndex in itemsToRemoveIndexes)
        {
            Debug.Log("Using ingredient " + itemList[itemIndex].itemData.name);
            inventory.RemoveItemAtIndex(itemIndex);
        }

        inventory.AddItem(craftingRecipe.resultItem);
    }
}
