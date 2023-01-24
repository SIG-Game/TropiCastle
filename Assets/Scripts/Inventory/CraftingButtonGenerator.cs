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

        Vector3 craftingButtonPosition = craftingButtonsParentTransform.position;

        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            GameObject craftingButton = Instantiate(craftingButtonPrefab, craftingButtonPosition, Quaternion.identity);
            craftingButton.name = $"Craft {craftingRecipes[i].name} Button";

            RectTransform craftingButtonRectTransform = craftingButton.GetComponent<RectTransform>();
            craftingButtonRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            craftingButtonRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            craftingButton.transform.SetParent(craftingButtonsParentTransform);

            craftingButton.GetComponent<CraftingButton>().SetUpCraftingButton(crafting, craftingRecipes[i]);

            craftingButtonPosition.y += yDistanceBetweenButtons;
        }
    }
}
