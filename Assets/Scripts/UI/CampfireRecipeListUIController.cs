using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CampfireRecipeListUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeListText;

    private void Awake()
    {
        var campfireRecipesLoadHandle =
            Addressables.LoadAssetsAsync<CampfireRecipeScriptableObject>(
                "campfire recipe", null);

        IList<CampfireRecipeScriptableObject> campfireRecipes =
            campfireRecipesLoadHandle.WaitForCompletion();

        Addressables.Release(campfireRecipesLoadHandle);

        var recipeListBuilder = new StringBuilder();

        foreach (CampfireRecipeScriptableObject recipe in campfireRecipes)
        {
            foreach (ItemStackStruct inputItemStruct in recipe.PossibleInputItems)
            {
                ItemStack inputItem = inputItemStruct.ToClassType();
                ItemStack resultItem = recipe.ResultItem.ToClassType();

                recipeListBuilder.AppendLine($"{inputItem} -> {resultItem}");
            }
        }

        recipeListText.text = recipeListBuilder.ToString();
    }
}
