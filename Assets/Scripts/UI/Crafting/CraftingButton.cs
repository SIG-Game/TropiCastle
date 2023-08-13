using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour, IElementWithMultiTextTooltip
{
    [SerializeField] private Button craftingButton;
    [SerializeField] private CraftingButtonDependencies craftingButtonDependencies;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private Image craftingButtonImage;

    private InventoryUIHeldItemController inventoryUIHeldItemController;
    private string resultItemTooltipText;

    private void Awake()
    {
        inventoryUIHeldItemController =
            craftingButtonDependencies.GetInventoryUIHeldItemController();

        // This could be changed to not be set at runtime
        // It this wasn't set at runtime, an old item tooltip format might get cached
        resultItemTooltipText = $"Result:\n" +
            craftingRecipe.resultItem.itemData.GetTooltipText();
    }

    private void Update()
    {
        craftingButton.interactable =
            !inventoryUIHeldItemController.HoldingItem();
    }

    public void CraftingButton_OnClick()
    {
        if (!inventoryUIHeldItemController.HoldingItem())
        {
            craftingButtonDependencies.GetCrafting().CraftItem(craftingRecipe);
        }
    }

    public void SetUpCraftingButton(CraftingButtonDependencies craftingButtonDependencies,
        CraftingRecipeScriptableObject craftingRecipe)
    {
        this.craftingButtonDependencies = craftingButtonDependencies;
        this.craftingRecipe = craftingRecipe;

        craftingButtonImage.sprite = craftingRecipe.resultItem.itemData.sprite;
    }

    private string GetIngredientsAsString()
    {
        StringBuilder ingredientsStringBuilder = new StringBuilder("Ingredients:\n");

        List<ItemWithAmount> playerInventoryItemList =
            craftingButtonDependencies.GetPlayerInventory().GetItemList();

        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        foreach (ItemWithAmount ingredient in craftingRecipe.ingredients)
        {
            bool playerHasIngredient = craftingButtonDependencies.GetCrafting()
                .TryFindIngredient(playerInventoryItemList, itemIndexToUsedAmount, ingredient);

            ingredientsStringBuilder.Append(playerHasIngredient ? "<color=#00FF00>" : "<color=#FF0000>");
            ingredientsStringBuilder.Append($"- {ingredient.amount} {ingredient.itemData.name}: ");
            ingredientsStringBuilder.Append(playerHasIngredient ? "Y" : "N");
            ingredientsStringBuilder.Append("</color>");
            ingredientsStringBuilder.AppendLine();
        }

        return ingredientsStringBuilder.ToString();
    }

    public string GetTooltipText() => GetIngredientsAsString();

    public string GetAlternateTooltipText() => resultItemTooltipText;
}
