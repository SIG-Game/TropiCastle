using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CampfireUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> campfireUIGameObjects;
    [SerializeField] private InventoryUIController campfireInventoryUIController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    private IList<CampfireRecipeScriptableObject> campfireRecipes;
    private Inventory campfireInventory;
    private bool cooking;

    private void Awake()
    {
        var campfireRecipesLoadHandle =
            Addressables.LoadAssetsAsync<CampfireRecipeScriptableObject>(
                "campfire recipe", null);

        campfireRecipes = campfireRecipesLoadHandle.WaitForCompletion();

        Addressables.Release(campfireRecipesLoadHandle);
    }

    public void Show()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(campfireUIGameObjects);
        inventoryUIManager.SetCanCloseUsingInteractAction(true);
        inventoryUIManager.EnableCurrentInventoryUI();

        inventoryUIManager.OnInventoryUIClosed += OnCampfireUIClosed;
    }

    public void SetInventory(Inventory campfireInventory)
    {
        this.campfireInventory = campfireInventory;

        campfireInventoryUIController.SetInventory(campfireInventory);

        campfireInventory.OnItemChangedAtIndex +=
            CampfireInventory_OnItemChangedAtIndex;
    }

    private bool TryCookItem()
    {
        ItemStack inputItem = campfireInventory.GetItemAtIndex(0);
        ItemStack inventoryResultItem = campfireInventory.GetItemAtIndex(1);

        CampfireRecipeScriptableObject inputItemCampfireRecipe = null;
        ItemStack recipeInputItem = null;

        IEnumerable<CampfireRecipeScriptableObject> possiblyMatchingRecipes;

        if (inventoryResultItem.itemDefinition.name == "Empty")
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

        foreach (var campfireRecipe in possiblyMatchingRecipes)
        {
            recipeInputItem = campfireRecipe.PossibleInputItems.FirstOrDefault(
                x => x.itemDefinition.name == inputItem.itemDefinition.name &&
                x.amount <= inputItem.amount);

            if (recipeInputItem != null)
            {
                inputItemCampfireRecipe = campfireRecipe;

                break;
            }
        }

        if (inputItemCampfireRecipe == null ||
            !playerInventory.CanAddItem(inputItemCampfireRecipe.ResultItem))
        {
            return false;
        }

        campfireInventory.SetItemAmountAtIndex(
            inputItem.amount - recipeInputItem.amount, 0);

        campfireInventory.AddItemAtIndex(
            inputItemCampfireRecipe.ResultItem, 1);

        return true;
    }

    private void OnCampfireUIClosed()
    {
        inventoryUIManager.OnInventoryUIClosed -= OnCampfireUIClosed;
        campfireInventory.OnItemChangedAtIndex -=
            CampfireInventory_OnItemChangedAtIndex;

        campfireInventory = null;
    }

    private void CampfireInventory_OnItemChangedAtIndex(ItemStack itemStack, int index)
    {
        if (cooking)
        {
            return;
        }

        cooking = true;

        while (TryCookItem()) {}

        cooking = false;
    }
}
