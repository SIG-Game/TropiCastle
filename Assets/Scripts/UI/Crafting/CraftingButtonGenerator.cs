using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingButtonGenerator : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private GameObject craftingButtonPrefab;
    [SerializeField] private Transform craftingButtonsParentTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private int yDistanceBetweenButtons;

    [ContextMenu("Generate Crafting Buttons")]
    private void GenerateCraftingButtons()
    {
        CraftingRecipeScriptableObject[] craftingRecipes = Resources.LoadAll<CraftingRecipeScriptableObject>("Crafting Recipes");

        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            Object craftingButton = PrefabUtility.InstantiatePrefab(craftingButtonPrefab,
                craftingButtonsParentTransform);
            craftingButton.name = $"Craft {craftingRecipes[i].name} Button";

            GameObject craftingButtonGameObject = craftingButton as GameObject;
            craftingButtonGameObject.GetComponent<CraftingButton>()
                .SetUpCraftingButton(crafting, playerInventory, craftingRecipes[i]);
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
