using System.Collections.Generic;
using System.IO;
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

    [MenuItem("Save Utility/Verify Save GUID Uniqueness in Active Scene")]
    public static void VerifySaveGuidUniquenessInActiveScene()
    {
        SaveManager[] saveManagers = Object.FindObjectsOfType<SaveManager>(true);

        var saveGuidToGameObjectName = new Dictionary<string, string>();

        bool duplicateSaveGuidExists = false;

        foreach (var saveManager in saveManagers)
        {
            string currentSaveGuid = saveManager.GetSaveGuid();

            if (saveGuidToGameObjectName.ContainsKey(currentSaveGuid))
            {
                duplicateSaveGuidExists = true;

                Debug.LogError($"Save GUID of GameObject {saveManager.gameObject.name} " +
                    $"is a duplicate of save GUID of GameObject " +
                    saveGuidToGameObjectName[currentSaveGuid]);
            }
            else
            {
                saveGuidToGameObjectName.Add(currentSaveGuid, saveManager.gameObject.name);
            }
        }

        if (!duplicateSaveGuidExists)
        {
            Debug.Log("No duplicate save GUIDs in active scene");
        }
    }

    [MenuItem("Save Utility/Delete Save Data")]
    public static void DeleteSaveFile()
    {
        string saveDataFilePath = SaveController.GetSaveDataFilePath();

        if (File.Exists(saveDataFilePath))
        {
            File.Delete(saveDataFilePath);

            Debug.Log("Save data file deleted");
        }
        else
        {
            Debug.Log("Save data file does not exist");
        }
    }
}
