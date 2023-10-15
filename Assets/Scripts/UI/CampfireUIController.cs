using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CampfireUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> campfireUIGameObjects;
    [SerializeField] private Button cookSelectedItemButton;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    private IList<CampfireRecipeScriptableObject> campfireRecipes;

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

    public void CookSelectedItemButton_OnClick()
    {
        ItemStack selectedItem = playerInventory.GetItemAtIndex(
            itemSelectionController.SelectedItemIndex);

        CampfireRecipeScriptableObject selectedItemCampfireRecipe = null;
        ItemStack recipeInputItem = null;

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

        playerInventory.ReplaceItems(
            new List<ItemStack> { recipeInputItem },
            selectedItemCampfireRecipe.ResultItem);
    }

    private void InventoryUIHeldItemController_OnItemHeld()
    {
        cookSelectedItemButton.interactable = false;
    }

    private void InventoryUIHeldItemController_OnHidden()
    {
        cookSelectedItemButton.interactable = true;
    }
}
