using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public abstract class SaveManager : MonoBehaviour
{
    [SerializeField] protected string saveGuid;

    public abstract SaveManagerState GetState();

    public abstract void UpdateFromState(SaveManagerState saveManagerState);

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
