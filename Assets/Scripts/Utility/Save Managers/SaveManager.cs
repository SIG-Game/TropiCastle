using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public abstract class SaveManager : MonoBehaviour
{
    [SerializeField] protected string saveGuid;

    public SaveManagerState GetState()
    {
        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = GetProperties()
        };

        return saveManagerState;
    }

    public abstract Dictionary<string, object> GetProperties();

    public abstract void UpdateFromProperties(Dictionary<string, object> properties);

    public string GetSaveGuid() => saveGuid;

#if UNITY_EDITOR
    [ContextMenu("Set Save GUID")]
    public void SetSaveGuid()
    {
        saveGuid = Guid.NewGuid().ToString();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
#endif
}
