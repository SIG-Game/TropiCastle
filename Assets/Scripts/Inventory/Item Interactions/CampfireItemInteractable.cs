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

        CampfireRecipeScriptableObject selectedItemCampfireRecipe = null;
        ItemWithAmount recipeInputItem = null;

        foreach (var campfireRecipe in campfireRecipes)
        {
            recipeInputItem = campfireRecipe.PossibleInputItems.FirstOrDefault(
                x => x.itemDefinition.name == selectedItem.itemDefinition.name);

            if (recipeInputItem != null)
            {
                selectedItemCampfireRecipe = campfireRecipe;

                break;
            }
        }

        if (selectedItemCampfireRecipe == null)
        {
            return;
        }

        Inventory playerInventory = playerController.GetInventory();

        playerInventory.ReplaceItems(
            new List<ItemWithAmount> { recipeInputItem },
            selectedItemCampfireRecipe.ResultItem);
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
    }
}
