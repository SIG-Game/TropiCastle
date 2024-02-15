using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CampfireItemInteractable : ContainerItemInteractable
{
    private IList<CampfireRecipeScriptableObject> campfireRecipes;
    private CampfireRecipeScriptableObject currentRecipe;
    private ItemStack? currentRecipeInputItem;
    private bool activeInUI;

    [Inject] private CampfireUIController campfireUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();

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
            float cookTimeProgress = itemInstanceProperties
                .GetFloatProperty("CookTimeProgress");

            cookTimeProgress += Time.unscaledDeltaTime;

            if (cookTimeProgress >= currentRecipe.CookTime)
            {
                ItemStack inputItem = inventory.GetItemAtIndex(0);

                int inputRemoveAmount = currentRecipeInputItem.Value.Amount;

                // Inventory_OnItemChangedAtIndex in this class
                // updates currentRecipe and currentRecipeInputItem
                // based on inventory changes

                inventory.AddItemAtIndex(currentRecipe.ResultItem, 1);

                inventory.SetItemAmountAtIndex(
                    inputItem.Amount - inputRemoveAmount, 0);

                cookTimeProgress = 0f;
            }

            itemInstanceProperties.SetProperty(
                "CookTimeProgress", cookTimeProgress);

            UpdateCampfireUIProgressArrow();
        }
    }

    public override void Interact()
    {
        campfireUIController.SetInventory(inventory);

        campfireUIController.Show();

        activeInUI = true;

        UpdateCampfireUIProgressArrow();

        campfireUIController.OnCampfireUIClosed +=
            CampfireUIController_OnCampfireUIClosed;
    }

    private void SetCurrentRecipe()
    {
        ItemStack inputItem = inventory.GetItemAtIndex(0);
        ItemStack inventoryResultItem = inventory.GetItemAtIndex(1);

        IEnumerable<CampfireRecipeScriptableObject> possiblyMatchingRecipes;

        if (inventoryResultItem.ItemDefinition.IsEmpty())
        {
            possiblyMatchingRecipes = campfireRecipes;
        }
        else
        {
            Func<CampfireRecipeScriptableObject, bool> isValidRecipe = x =>
                x.ResultItem.ItemDefinition.name ==
                    inventoryResultItem.ItemDefinition.name &&
                x.ResultItem.Amount + inventoryResultItem.Amount <=
                    x.ResultItem.ItemDefinition.StackSize;

            possiblyMatchingRecipes = campfireRecipes.Where(isValidRecipe);
        }

        CampfireRecipeScriptableObject matchingRecipe = null;
        ItemStack? matchingRecipeInputItem = null;

        foreach (var recipe in possiblyMatchingRecipes)
        {
            int inputItemIndex = recipe.PossibleInputItems.FindIndex(
                x => x.ItemDefinition.name == inputItem.ItemDefinition.name &&
                x.Amount <= inputItem.Amount);

            if (inputItemIndex != -1)
            {
                matchingRecipeInputItem =
                    recipe.PossibleInputItems[inputItemIndex];
                matchingRecipe = recipe;

                break;
            }
        }

        if (matchingRecipe != null)
        {
            currentRecipe = matchingRecipe;
            currentRecipeInputItem = matchingRecipeInputItem.Value;
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

    private void ResetCookTimeProgress()
    {
        itemInstanceProperties.SetProperty("CookTimeProgress", 0f);

        UpdateCampfireUIProgressArrow();
    }

    protected override void Inventory_OnItemChangedAtIndex(ItemStack item, int index)
    {
        base.Inventory_OnItemChangedAtIndex(item, index);

        if (currentRecipe == null || currentRecipeInputItem == null)
        {
            SetCurrentRecipe();
            ResetCookTimeProgress();
        }
        else
        {
            ItemStack inputItem = inventory.GetItemAtIndex(0);
            ItemStack inventoryResultItem = inventory.GetItemAtIndex(1);

            bool inputItemMatchesCurrentRecipe =
                inputItem.ItemDefinition == currentRecipeInputItem.Value.ItemDefinition &&
                inputItem.Amount >= currentRecipeInputItem.Value.Amount;
            bool inventoryResultItemMatchesCurrentRecipe = 
                inventoryResultItem.ItemDefinition.IsEmpty() ||
                (inventoryResultItem.ItemDefinition == currentRecipe.ResultItem.ItemDefinition &&
                inventoryResultItem.Amount + currentRecipe.ResultItem.Amount <=
                    inventoryResultItem.ItemDefinition.StackSize);
            if (!inputItemMatchesCurrentRecipe ||
                !inventoryResultItemMatchesCurrentRecipe)
            {
                SetCurrentRecipe();
                ResetCookTimeProgress();
            }
        }
    }

    private void CampfireUIController_OnCampfireUIClosed()
    {
        activeInUI = false;

        campfireUIController.OnCampfireUIClosed -=
            CampfireUIController_OnCampfireUIClosed;
    }
}
