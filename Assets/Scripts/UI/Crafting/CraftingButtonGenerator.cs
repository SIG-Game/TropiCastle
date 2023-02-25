using UnityEngine;

public class CraftingButtonGenerator : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private GameObject craftingButtonPrefab;
    [SerializeField] private Transform craftingButtonsParentTransform;
    [SerializeField] private int yDistanceBetweenButtons;

    private void Awake()
    {
        CraftingRecipeScriptableObject[] craftingRecipes = Resources.LoadAll<CraftingRecipeScriptableObject>("Crafting Recipes");

        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            GameObject craftingButton = Instantiate(craftingButtonPrefab, craftingButtonsParentTransform);
            craftingButton.name = $"Craft {craftingRecipes[i].name} Button";

            craftingButton.GetComponent<CraftingButton>().SetUpCraftingButton(crafting, craftingRecipes[i]);
        }
    }
}
