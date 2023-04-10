using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CraftingButtonGenerator : MonoBehaviour
{
    [SerializeField] private CraftingButtonDependencies craftingButtonDependencies;
    [SerializeField] private GameObject craftingButtonPrefab;
    [SerializeField] private Transform craftingButtonsParentTransform;

    [ContextMenu("Regenerate Crafting Buttons")]
    private void RegenerateCraftingButtons()
    {
        string prefabAssetPath;

        PrefabStage currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();

        bool inPrefabMode = currentPrefabStage != null;
        if (inPrefabMode)
        {
            prefabAssetPath = currentPrefabStage.assetPath;
        }
        else
        {
            prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabAssetPath);

        CraftingButtonGenerator prefabCraftingButtonGenerator =
            prefabRoot.GetComponent<CraftingButtonGenerator>();

        Transform prefabCraftingButtonsParent =
            prefabCraftingButtonGenerator.craftingButtonsParentTransform;

        DeleteCraftingButtons(prefabCraftingButtonsParent);

        CraftingRecipeScriptableObject[] craftingRecipes =
            Resources.LoadAll<CraftingRecipeScriptableObject>("Crafting Recipes");

        // Apply changes to prefab
        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            GameObject craftingButton = PrefabUtility.InstantiatePrefab(craftingButtonPrefab,
                prefabCraftingButtonsParent) as GameObject;
            craftingButton.name = $"Craft {craftingRecipes[i].name} Button";

            craftingButton.GetComponent<CraftingButton>().SetUpCraftingButton(
                prefabCraftingButtonGenerator.craftingButtonDependencies, craftingRecipes[i]);
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabAssetPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private void DeleteCraftingButtons(Transform prefabCraftingButtonsParent)
    {
        // Delete crafting buttons from prefab
        while (prefabCraftingButtonsParent.childCount > 0)
        {
            DestroyImmediate(prefabCraftingButtonsParent.GetChild(0).gameObject);
        }

        // Crafting buttons are deleted from the prefab instance automatically
    }
}
