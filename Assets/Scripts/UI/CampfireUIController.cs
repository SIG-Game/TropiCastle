using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CampfireUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> campfireUIGameObjects;
    [SerializeField] private InventoryUIController campfireInventoryUIController;
    [SerializeField] private Button cookItemButton;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    private IList<CampfireRecipeScriptableObject> campfireRecipes;
    private Inventory campfireInventory;

    private void Awake()
    {
        var campfireRecipesLoadHandle =
            Addressables.LoadAssetsAsync<CampfireRecipeScriptableObject>(
                "campfire recipe", null);

        campfireRecipes = campfireRecipesLoadHandle.WaitForCompletion();

        Addressables.Release(campfireRecipesLoadHandle);

        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden +=
            InventoryUIHeldItemController_OnHidden;
    }

    private void OnDestroy()
    {
        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -=
            InventoryUIHeldItemController_OnHidden;
    }

    public void Show()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(campfireUIGameObjects);
        inventoryUIManager.SetCanCloseUsingInteractAction(true);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    public void SetInventory(Inventory campfireInventory)
    {
        this.campfireInventory = campfireInventory;

        campfireInventoryUIController.SetInventory(campfireInventory);
    }

    public void CookItemButton_OnClick()
    {
        ItemStack inputItem = campfireInventory.GetItemAtIndex(0);

        CampfireRecipeScriptableObject inputItemCampfireRecipe = null;
        ItemStack recipeInputItem = null;

        foreach (var campfireRecipe in campfireRecipes)
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
            return;
        }

        campfireInventory.SetItemAmountAtIndex(
            inputItem.amount - recipeInputItem.amount, 0);

        playerInventory.AddItem(inputItemCampfireRecipe.ResultItem);
    }

    private void InventoryUIHeldItemController_OnItemHeld()
    {
        cookItemButton.interactable = false;
    }

    private void InventoryUIHeldItemController_OnHidden()
    {
        cookItemButton.interactable = true;
    }
}
