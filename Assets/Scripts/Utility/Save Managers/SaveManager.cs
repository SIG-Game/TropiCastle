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

    public abstract SaveManagerState GetState();

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
