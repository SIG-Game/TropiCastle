using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour, IElementWithTooltip
{
    [SerializeField] private Button craftingButton;
    [SerializeField] private CraftingButtonDependencies craftingButtonDependencies;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private Image craftingButtonImage;

    private Inventory playerInventory;
    private InventoryUIHeldItemController inventoryUIHeldItemController;
    private string resultItemTooltipText;

    private void Awake()
    {
        playerInventory = craftingButtonDependencies.PlayerInventory;
        inventoryUIHeldItemController =
            craftingButtonDependencies.InventoryUIHeldItemController;

        // This could be changed to not be set at runtime
        // It this wasn't set at runtime, an old item tooltip format might get cached
        resultItemTooltipText = $"Result:\n" +
            craftingRecipe.ResultItem.ItemDefinition.GetTooltipText();

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

    public void CraftingButton_OnClick()
    {
        playerInventory.ReplaceItems(
            craftingRecipe.Ingredients.Select(x => x.ToClassType()).ToList(),
            craftingRecipe.ResultItem.ToClassType());
    }

    public void SetUpCraftingButton(CraftingButtonDependencies craftingButtonDependencies,
        CraftingRecipeScriptableObject craftingRecipe)
    {
        this.craftingButtonDependencies = craftingButtonDependencies;
        this.craftingRecipe = craftingRecipe;

        craftingButtonImage.sprite = craftingRecipe.ResultItem.ItemDefinition.Sprite;
    }

    private string GetIngredientsAsString()
    {
        StringBuilder ingredientsStringBuilder = new StringBuilder("Ingredients:\n");

        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        foreach (ItemStackStruct ingredientStruct in craftingRecipe.Ingredients)
        {
            ItemStack ingredient = ingredientStruct.ToClassType();

            bool playerHasIngredient = playerInventory
                .HasReplacementInputItem(itemIndexToUsedAmount, ingredient);

            ingredientsStringBuilder.Append(
                playerHasIngredient ? "<color=#00FF00>" : "<color=#FF0000>");
            ingredientsStringBuilder.Append($"- {ingredient}: ");
            ingredientsStringBuilder.Append(playerHasIngredient ? "Y" : "N");
            ingredientsStringBuilder.Append("</color>");
            ingredientsStringBuilder.AppendLine();
        }

        return ingredientsStringBuilder.ToString();
    }

    public string GetTooltipText() => GetIngredientsAsString();

    public string GetAlternateTooltipText() => resultItemTooltipText;

    private void InventoryUIHeldItemController_OnItemHeld()
    {
        craftingButton.interactable = false;
    }

    private void InventoryUIHeldItemController_OnHidden()
    {
        craftingButton.interactable = true;
    }
}
