using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class EditorPlayerController : Editor
{
    private int selectedIndex;
    private ItemScriptableObject[] itemScriptableObjects;
    private string[] options;

    private void OnEnable()
    {
        selectedIndex = 0;
        itemScriptableObjects = Resources.LoadAll<ItemScriptableObject>("Items");
        options = itemScriptableObjects.Select(x => x.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Inventory Debug Controls", style: EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Item Type");
        selectedIndex = EditorGUILayout.Popup(selectedIndex, options);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add Item"))
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Cannot add item when not in play mode");
                return;
            }

            ItemScriptableObject itemScriptableObjectToAdd = itemScriptableObjects[selectedIndex];
            Inventory inventory = ((PlayerController)target).GetInventory();
            inventory.AddItem(itemScriptableObjectToAdd, 1);
        }
    }
}
