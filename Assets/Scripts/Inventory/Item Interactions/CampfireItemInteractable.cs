using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

public class CampfireItemInteractable : Interactable
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

        bool inputItemSlotWillBeEmptied = selectedItem.amount == 1;
        if (inputItemSlotWillBeEmptied || playerInventory.CanAddItem(resultItem))
        {
            int selectedItemIndex = playerController.GetSelectedItemIndex();

            playerInventory.DecrementItemStackAtIndex(selectedItemIndex);

            playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(
                resultItem, selectedItemIndex);
        }
        else
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
        }
    }
}
