using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private TextMeshProUGUI craftingButtonText;
    [SerializeField] private Image craftingButtonImage;
    [SerializeField] private IngredientsTooltipController ingredientsTooltipController;

    public void CraftingButton_OnClick()
    {
        crafting.CraftItem(craftingRecipe);
    }

    public void SetUpCraftingButton(Crafting crafting, CraftingRecipeScriptableObject craftingRecipe,
        IngredientsTooltipController ingredientsTooltipController)
    {
        this.crafting = crafting;
        this.craftingRecipe = craftingRecipe;
        this.ingredientsTooltipController = ingredientsTooltipController;

        craftingButtonText.text = $"Craft {craftingRecipe.name}";
        craftingButtonImage.sprite = craftingRecipe.resultItem.itemData.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ingredientsTooltipController.ShowTooltipWithText(GetIngredientsAsString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ingredientsTooltipController.HideTooltip();
    }

    private string GetIngredientsAsString()
    {
        StringBuilder ingredientsStringBuilder = new StringBuilder("Ingredients:\n");

        foreach (ItemWithAmount ingredient in craftingRecipe.ingredients)
        {
            ingredientsStringBuilder.AppendLine("- " + ingredient.itemData.name);
        }

        return ingredientsStringBuilder.ToString();
    }
}
