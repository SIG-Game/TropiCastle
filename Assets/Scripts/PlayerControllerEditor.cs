using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    int selectedIndex = 0;
    String[] options;
    Inventory inventory;

    private void OnEnable()
    {
        options = Enum.GetNames(typeof(Item.ItemType));
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
            inventory.AddItem((Item.ItemType)selectedIndex, 1);
        }
    }
}
