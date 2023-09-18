using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public abstract class SaveManager : MonoBehaviour, ISavable
{
    [SerializeField] protected string saveGuid;

    public abstract SavableState GetSavableState();

    public abstract void SetPropertiesFromSavableState(SavableState savableState);

    public string GetSaveGuid() => saveGuid;

#if UNITY_EDITOR
    [ContextMenu("Set Save GUID")]
    private void SetSaveGuid()
    {
        saveGuid = Guid.NewGuid().ToString();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
#endif
}
