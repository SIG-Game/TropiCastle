using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CampfireItemInteractable : ItemInteractable
{
    private Inventory campfireInventory;
    private CampfireItemInstanceProperties campfireItemInstanceProperties;
    private CampfireUIController campfireUIController;
    private IList<CampfireRecipeScriptableObject> campfireRecipes;
    private CampfireRecipeScriptableObject currentRecipe;
    private ItemStack currentRecipeInputItem;

    private void Awake()
    {
        campfireInventory = gameObject.AddComponent<Inventory>();

        campfireItemInstanceProperties =
            (CampfireItemInstanceProperties)GetComponent<ItemWorld>()
                .GetItem().instanceProperties;

        campfireInventory.InitializeItemListWithSize(
            CampfireItemInstanceProperties.CampfireInventorySize);

        if (campfireItemInstanceProperties != null)
        {
            campfireInventory.SetUpFromSerializableInventory(
                campfireItemInstanceProperties.SerializableInventory);
        }

        var campfireRecipesLoadHandle =
            Addressables.LoadAssetsAsync<CampfireRecipeScriptableObject>(
                "campfire recipe", null);

        campfireRecipes = campfireRecipesLoadHandle.WaitForCompletion();

        Addressables.Release(campfireRecipesLoadHandle);

        SetCurrentRecipe();

        campfireInventory.OnItemChangedAtIndex +=
            CampfireInventory_OnItemChangedAtIndex;
    }

    private void Update()
    {
        if (currentRecipe != null)
        {
            campfireItemInstanceProperties.CookTimeProgress += Time.unscaledDeltaTime;

            if (campfireItemInstanceProperties.CookTimeProgress >= currentRecipe.CookTime)
            {
                ItemStack inputItem = campfireInventory.GetItemAtIndex(0);

                campfireInventory.AddItemAtIndex(currentRecipe.ResultItem, 1);

                // Changing the input item in campfireInventory
                // updates currentRecipe and currentRecipeInputItem
                campfireInventory.SetItemAmountAtIndex(
                    inputItem.amount - currentRecipeInputItem.amount, 0);

                campfireItemInstanceProperties.CookTimeProgress = 0f;
            }
        }
    }

    private void OnDestroy()
    {
        campfireInventory.OnItemChangedAtIndex -=
            CampfireInventory_OnItemChangedAtIndex;
    }

    public override void Interact(PlayerController _)
    {
        campfireUIController.SetInventory(campfireInventory);

        campfireUIController.Show();
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        campfireUIController =
            itemInteractableDependencies.GetCampfireUIController();
    }

    private void SetCurrentRecipe()
    {
        ItemStack inputItem = campfireInventory.GetItemAtIndex(0);
        ItemStack inventoryResultItem = campfireInventory.GetItemAtIndex(1);

        IEnumerable<CampfireRecipeScriptableObject> possiblyMatchingRecipes;

        if (inventoryResultItem.itemDefinition.IsEmpty())
        {
            possiblyMatchingRecipes = campfireRecipes;
        }
        else
        {
            Func<CampfireRecipeScriptableObject, bool> isValidRecipe = x =>
                x.ResultItem.itemDefinition.name ==
                    inventoryResultItem.itemDefinition.name &&
                x.ResultItem.amount + inventoryResultItem.amount <=
                    x.ResultItem.itemDefinition.stackSize;

            possiblyMatchingRecipes = campfireRecipes.Where(isValidRecipe);
        }

        CampfireRecipeScriptableObject matchingRecipe = null;
        ItemStack matchingRecipeInputItem = null;

        foreach (var recipe in possiblyMatchingRecipes)
        {
            matchingRecipeInputItem = recipe.PossibleInputItems.FirstOrDefault(
                x => x.itemDefinition.name == inputItem.itemDefinition.name &&
                x.amount <= inputItem.amount);

            if (matchingRecipeInputItem != null)
            {
                matchingRecipe = recipe;

                break;
            }
        }

        if (matchingRecipe != null)
        {
            currentRecipe = matchingRecipe;
            currentRecipeInputItem = matchingRecipeInputItem;
        }
        else
        {
            currentRecipe = null;
            currentRecipeInputItem = null;
        }
    }

    private void CampfireInventory_OnItemChangedAtIndex(ItemStack itemStack, int index)
    {
        campfireItemInstanceProperties.UpdateSerializableInventory(campfireInventory);

        bool inputItemChanged = index == 0;
        if (inputItemChanged)
        {
            SetCurrentRecipe();

            campfireItemInstanceProperties.CookTimeProgress = 0f;
        }
    }
}
