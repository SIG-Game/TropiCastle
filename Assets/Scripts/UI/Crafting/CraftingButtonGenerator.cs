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

    [ContextMenu("Generate Crafting Buttons")]
    private void GenerateCraftingButtons()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            Debug.Log("This command cannot be used in Prefab Mode. " +
                "Use this command on a prefab instance in a scene instead.");
            return;
        }

        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

        CraftingRecipeScriptableObject[] craftingRecipes = Resources.LoadAll<CraftingRecipeScriptableObject>("Crafting Recipes");

        // Apply changes to prefab
        for (int i = 0; i < craftingRecipes.Length; ++i)
        {
            Object craftingButton = PrefabUtility.InstantiatePrefab(craftingButtonPrefab,
                prefabRoot.GetComponent<CraftingButtonGenerator>().craftingButtonsParentTransform);
            craftingButton.name = $"Craft {craftingRecipes[i].name} Button";
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(prefabRoot);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
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
}
