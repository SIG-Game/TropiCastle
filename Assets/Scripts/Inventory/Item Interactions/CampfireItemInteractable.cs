using System;
using System.Collections.Generic;
using UnityEngine;

public class CampfireItemInteractable : Interactable
{
    private static readonly Lazy<ItemWithAmount> lazyCookedCrabMeat =
        new Lazy<ItemWithAmount>(() =>
            new ItemWithAmount(LoadItemScriptableObject("CookedCrabMeat"), 1));

    private static List<CampfireRecipe> campfireRecipes = new List<CampfireRecipe>
    {
        new CampfireRecipe("Raw Crab Meat", lazyCookedCrabMeat)
    };

    public override void Interact(PlayerController playerController)
    {
        Inventory playerInventory = playerController.GetInventory();
        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        bool showInventoryFullText = false;

        foreach (CampfireRecipe campfireRecipe in campfireRecipes)
        {
            int inputItemIndex = playerInventoryItemList
                .FindIndex(x => x.itemData.name == campfireRecipe.InputItemName);

            if (inputItemIndex == -1)
            {
                continue;
            }

            ItemWithAmount inputItem = playerInventoryItemList[inputItemIndex];
            ItemWithAmount resultItem = campfireRecipe.LazyResultItem.Value;

            bool inputItemSlotEmptied = inputItem.amount == 1;

            if (inputItemSlotEmptied || playerInventory.CanAddItem(resultItem))
            {
                playerInventory.DecrementItemStackAtIndex(inputItemIndex);

                playerInventory.AddItem(resultItem);

                showInventoryFullText = false;

                break;
            }
            else
            {
                showInventoryFullText = true;
            }
        }

        if (showInventoryFullText)
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
        }
    }

    private static ItemScriptableObject LoadItemScriptableObject(string name) =>
        Resources.Load<ItemScriptableObject>($"Items/{name}");

    private class CampfireRecipe
    {
        public string InputItemName;
        public Lazy<ItemWithAmount> LazyResultItem;

        public CampfireRecipe(string inputItemName, Lazy<ItemWithAmount> lazyResultItem)
        {
            InputItemName = inputItemName;
            LazyResultItem = lazyResultItem;
        }
    }
}
