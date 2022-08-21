using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    int selectedIndex = 0;
    UnityEngine.Object[] itemScriptableObjects;
    String[] options;
    Inventory inventory;

    private void OnEnable()
    {
        // Get items
        itemScriptableObjects = Resources.LoadAll("Items", typeof(ItemScriptableObject));

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

            inventory = ((PlayerController)target).GetInventory();
            ItemScriptableObject foundItemScriptableObject = (ItemScriptableObject)itemScriptableObjects.Single(x => x.name == options[selectedIndex]);
            inventory.AddItem(foundItemScriptableObject, 1);
        }
    }
}
