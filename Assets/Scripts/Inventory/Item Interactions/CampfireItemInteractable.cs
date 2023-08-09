using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

public class CampfireItemInteractable : ItemInteractable
{
    private static IList<CampfireRecipeScriptableObject> campfireRecipes;

    private void Awake()
    {
        if (campfireRecipes == null)
        {
            var campfireRecipesLoadHandle =
                Addressables.LoadAssetsAsync<CampfireRecipeScriptableObject>(
                    "campfire recipe", null);

            campfireRecipes = campfireRecipesLoadHandle.WaitForCompletion();

            Addressables.Release(campfireRecipesLoadHandle);
        }
    }

    public override void Interact(PlayerController playerController)
    {
        ItemWithAmount selectedItem = playerController.GetSelectedItem();

        CampfireRecipeScriptableObject selectedItemCampfireRecipe =
            campfireRecipes.FirstOrDefault(
                x => x.inputItem.name == selectedItem.itemData.name);

        if (selectedItemCampfireRecipe == null)
        {
            return;
        }

        Inventory playerInventory = playerController.GetInventory();
        ItemWithAmount resultItem = selectedItemCampfireRecipe.resultItem;

        int selectedItemIndex = playerController.GetSelectedItemIndex();

        playerInventory.DecrementItemStackAtIndex(selectedItemIndex);

        playerInventory.TryAddItemAtIndexWithFallbackToFirstEmptyIndex(
            resultItem, selectedItemIndex, out int amountAdded);

        bool resultItemNotAdded = amountAdded == 0;
        if (resultItemNotAdded)
        {
            // This item stack is guaranteed to not be empty. If this item
            // stack was empty, then the result item would have been added
            // to the player's inventory.
            playerInventory.IncrementItemStackAtIndex(selectedItemIndex);
        }
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
    }
}
