using UnityEngine;

public class CraftingButton : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;

    public void CraftingButton_OnClick()
    {
        crafting.CraftItem(craftingRecipe);
    }
}
