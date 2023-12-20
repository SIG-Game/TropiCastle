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
            foreach (ItemStackStruct inputItem in recipe.PossibleInputItems)
            {
                recipeListBuilder.AppendLine($"{inputItem} -> {recipe.ResultItem}");
            }
        }

        recipeListText.text = recipeListBuilder.ToString();
    }
}
