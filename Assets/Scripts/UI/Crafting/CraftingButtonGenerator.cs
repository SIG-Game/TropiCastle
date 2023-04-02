using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButtonGenerator : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private GameObject craftingButtonPrefab;
    [SerializeField] private Transform craftingButtonsParentTransform;
    [SerializeField] private Inventory playerInventory;

    [ContextMenu("Regenerate Crafting Buttons")]
    private void RegenerateCraftingButtons()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            Debug.Log("This command cannot be used in Prefab Mode. " +
                "Use this command on a prefab instance in a scene instead.");
            return;
        }

        string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabAssetPath);

        Transform prefabCraftingButtonsParent =
            prefabRoot.GetComponent<CraftingButtonGenerator>().craftingButtonsParentTransform;

        DeleteCraftingButtons(prefabCraftingButtonsParent);

        CraftingRecipeScriptableObject[] craftingRecipes =
            Resources.LoadAll<CraftingRecipeScriptableObject>("Crafting Recipes");

        // Apply changes to prefab
        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            Object craftingButton = PrefabUtility.InstantiatePrefab(craftingButtonPrefab,
                prefabCraftingButtonsParent);
            craftingButton.name = $"Craft {craftingRecipes[i].name} Button";
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabAssetPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        // Apply changes to prefab instance
        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            GameObject instanceCraftingButtonGameObject =
                craftingButtonsParentTransform.GetChild(i).gameObject;

            instanceCraftingButtonGameObject.GetComponent<CraftingButton>()
                .SetUpCraftingButton(crafting, playerInventory, craftingRecipes[i]);

            EditorUtility.SetDirty(instanceCraftingButtonGameObject.GetComponent<CraftingButton>());
            EditorUtility.SetDirty(instanceCraftingButtonGameObject.transform
                .GetChild(0).GetComponent<Image>());
        }
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
