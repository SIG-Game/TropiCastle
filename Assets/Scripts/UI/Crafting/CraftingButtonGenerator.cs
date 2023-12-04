using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class CraftingButtonGenerator : MonoBehaviour
{
    [SerializeField] private CraftingButtonDependencies craftingButtonDependencies;
    [SerializeField] private GameObject craftingButtonPrefab;
    [SerializeField] private Transform craftingButtonsParentTransform;

#if UNITY_EDITOR
    private GameObject prefabRoot;
    private CraftingButtonGenerator prefabCraftingButtonGenerator;
    private Transform prefabCraftingButtonsParent;
    private string prefabAssetPath;

    [ContextMenu("Regenerate Crafting Buttons")]
    private void RegenerateCraftingButtons()
    {
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

        prefabRoot = PrefabUtility.LoadPrefabContents(prefabAssetPath);

        prefabCraftingButtonGenerator =
            prefabRoot.GetComponent<CraftingButtonGenerator>();

        prefabCraftingButtonsParent =
            prefabCraftingButtonGenerator.craftingButtonsParentTransform;

        while (prefabCraftingButtonsParent.childCount > 0)
        {
            DestroyImmediate(prefabCraftingButtonsParent.GetChild(0).gameObject);
        }

        var craftingRecipeScriptableObjectsLoadHandle =
            Addressables.LoadAssetsAsync<CraftingRecipeScriptableObject>("crafting recipe", null);

        craftingRecipeScriptableObjectsLoadHandle.Completed +=
            CraftingRecipeScriptableObjectsLoadHandle_Completed;

        // Crafting button regeneration is slow without this method call
        craftingRecipeScriptableObjectsLoadHandle.WaitForCompletion();
    }

    private void CraftingRecipeScriptableObjectsLoadHandle_Completed(
        AsyncOperationHandle<IList<CraftingRecipeScriptableObject>>
        craftingRecipesAsyncOperationHandle)
    {
        if (craftingRecipesAsyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"{nameof(CraftingRecipeScriptableObject)}s not loaded in " +
                $"{nameof(CraftingButtonGenerator)}.");

            Addressables.Release(craftingRecipesAsyncOperationHandle);

            return;
        }

        IList<CraftingRecipeScriptableObject> craftingRecipes =
            craftingRecipesAsyncOperationHandle.Result;

        // Apply changes to prefab
        foreach (CraftingRecipeScriptableObject craftingRecipe in craftingRecipes)
        {
            GameObject craftingButton = PrefabUtility.InstantiatePrefab(craftingButtonPrefab,
                prefabCraftingButtonsParent) as GameObject;
            craftingButton.name = $"{craftingRecipe.name} Button";

            craftingButton.GetComponent<CraftingButton>().SetUpCraftingButton(
                prefabCraftingButtonGenerator.craftingButtonDependencies, craftingRecipe);
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabAssetPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        Addressables.Release(craftingRecipesAsyncOperationHandle);
    }
#endif
}
