using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CampfireItemInteractable :
    ContainerItemInteractable<CampfireItemInstanceProperties>
{
    private CampfireUIController campfireUIController;
    private IList<CampfireRecipeScriptableObject> campfireRecipes;
    private CampfireRecipeScriptableObject currentRecipe;
    private ItemStack currentRecipeInputItem;
    private bool activeInUI;

    protected override void Awake()
    {
        base.Awake();

        var campfireRecipesLoadHandle =
            Addressables.LoadAssetsAsync<CampfireRecipeScriptableObject>(
                "campfire recipe", null);

        campfireRecipes = campfireRecipesLoadHandle.WaitForCompletion();

        Addressables.Release(campfireRecipesLoadHandle);

        SetCurrentRecipe();
    }

    private void Update()
    {
        if (currentRecipe != null)
        {
            float cookTimeProgress =
                itemInstanceProperties.GetFloatProperty("CookTimeProgress");

            cookTimeProgress += Time.unscaledDeltaTime;

            if (cookTimeProgress >= currentRecipe.CookTime)
            {
                ItemStack inputItem = inventory.GetItemAtIndex(0);

                inventory.AddItemAtIndex(currentRecipe.ResultItem, 1);

                // Changing the input item in campfireInventory
                // updates currentRecipe and currentRecipeInputItem
                inventory.SetItemAmountAtIndex(
                    inputItem.amount - currentRecipeInputItem.amount, 0);

                cookTimeProgress = 0f;
            }

            itemInstanceProperties.SetExistingProperty(
                "CookTimeProgress", cookTimeProgress.ToString());

            UpdateCampfireUIProgressArrow();
        }
    }

    public override void Interact(PlayerController _)
    {
        campfireUIController.SetInventory(inventory);

        campfireUIController.Show();

        activeInUI = true;

        UpdateCampfireUIProgressArrow();

        campfireUIController.OnCampfireUIClosed +=
            CampfireUIController_OnCampfireUIClosed;
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        campfireUIController =
            itemInteractableDependencies.GetCampfireUIController();
    }

    private void SetCurrentRecipe()
    {
        ItemStack inputItem = inventory.GetItemAtIndex(0);
        ItemStack inventoryResultItem = inventory.GetItemAtIndex(1);

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
                    x.ResultItem.itemDefinition.StackSize;

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

    private void UpdateCampfireUIProgressArrow()
    {
        if (activeInUI)
        {
            if (currentRecipe == null)
            {
                campfireUIController.HideCookTimeProgressArrow();
            }
            else
            {
                campfireUIController.UpdateCookTimeProgressArrow(
                    itemInstanceProperties.GetFloatProperty("CookTimeProgress"),
                    currentRecipe.CookTime);
            }
        }
    }

    protected override void Inventory_OnItemChangedAtIndex(ItemStack itemStack, int index)
    {
        base.Inventory_OnItemChangedAtIndex(itemStack, index);

        bool inputItemChanged = index == 0;
        bool resultItemEmptied =
            index == 1 && inventory.GetItemAtIndex(1).itemDefinition.IsEmpty();
        if (inputItemChanged || resultItemEmptied)
        {
            SetCurrentRecipe();

            itemInstanceProperties.SetExistingProperty("CookTimeProgress", "0");

            UpdateCampfireUIProgressArrow();
        }
    }

    private void CampfireUIController_OnCampfireUIClosed()
    {
        activeInUI = false;

        campfireUIController.OnCampfireUIClosed -=
            CampfireUIController_OnCampfireUIClosed;
    }
}
