using System.Collections.Generic;
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
        Inventory playerInventory = playerController.GetInventory();
        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        bool showInventoryFullText = false;

        foreach (CampfireRecipeScriptableObject campfireRecipe in campfireRecipes)
        {
            int inputItemIndex = playerInventoryItemList
                .FindIndex(x => x.itemData.name == campfireRecipe.inputItem.name);

            if (inputItemIndex == -1)
            {
                continue;
            }

            ItemWithAmount inputItem = playerInventoryItemList[inputItemIndex];
            ItemWithAmount resultItem = campfireRecipe.resultItem;

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
}
