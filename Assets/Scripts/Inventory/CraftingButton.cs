using TMPro;
using UnityEngine;

public class CraftingButton : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private TextMeshProUGUI craftingButtonText;

    public void CraftingButton_OnClick()
    {
        crafting.CraftItem(craftingRecipe);
    }

    public void SetUpCraftingButton(Crafting crafting, CraftingRecipeScriptableObject craftingRecipe)
    {
        this.crafting = crafting;
        this.craftingRecipe = craftingRecipe;

        craftingButtonText.text = $"Craft {craftingRecipe.name}";
    }
}
