using System;
using System.Collections.Generic;
using UnityEngine;

public class CampfireItemInteractable : Interactable
{
    private static readonly Lazy<ItemScriptableObject> lazyCookedCrabMeat =
        new Lazy<ItemScriptableObject>(() => LoadItemScriptableObject("CookedCrabMeat"));

    private static List<CampfireRecipe> campfireRecipes = new List<CampfireRecipe>
    {
        new CampfireRecipe("Raw Crab Meat", lazyCookedCrabMeat)
    };

    public override void Interact(PlayerController playerController)
    {
        Inventory playerInventory = playerController.GetInventory();

        foreach (CampfireRecipe campfireRecipe in campfireRecipes)
        {
            int inputItemIndex = playerInventory.GetItemList()
                .FindIndex(x => x.itemData.name == campfireRecipe.InputItemName);

            if (inputItemIndex != -1)
            {
                playerInventory.DecrementItemStackAtIndex(inputItemIndex);

                playerInventory.AddItem(campfireRecipe.LazyResultItem.Value, 1);
            }
        }
    }

    private static ItemScriptableObject LoadItemScriptableObject(string name) =>
        Resources.Load<ItemScriptableObject>($"Items/{name}");

    private class CampfireRecipe
    {
        public string InputItemName;
        public Lazy<ItemScriptableObject> LazyResultItem;

        public CampfireRecipe(string inputItemName, Lazy<ItemScriptableObject> lazyResultItem)
        {
            InputItemName = inputItemName;
            LazyResultItem = lazyResultItem;
        }
    }
}
