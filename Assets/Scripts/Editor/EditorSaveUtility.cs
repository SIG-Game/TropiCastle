using UnityEditor;
using UnityEngine;

public static class EditorSaveUtility
{
    [MenuItem("Save Utility/Fix Empty Save GUIDs in Active Scene")]
    public static void FixEmptySaveGuidsInActiveScene()
    {
        SaveManager[] saveManagers =
            Object.FindObjectsOfType<SaveManager>(true);

        bool emptySaveGuidExists = false;

        foreach (var saveManager in saveManagers)
        {
            if (string.IsNullOrWhiteSpace(saveManager.GetSaveGuid()))
            {
                emptySaveGuidExists = true;

                saveManager.SetSaveGuid();

                Debug.Log($"Set save GUID for {saveManager.gameObject.name}");
            }
        }

        if (!emptySaveGuidExists)
        {
            Debug.Log("No empty save GUIDs in active scene");
        }
    }
}
