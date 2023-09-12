using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class DebugAddItemUISaveManager : MonoBehaviour, ISavableNongeneric
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;
    [SerializeField] private string saveGuid;

    public SavableState GetSavableState()
    {
        var savableState = new SavableDebugAddItemUIState
        {
            SaveGuid = saveGuid,
            AmountInputFieldText = amountInputField.text,
            ItemDropdownValue = itemDropdown.value
        };

        return savableState;
    }

    public void SetPropertiesFromSavableState(SavableState savableState)
    {
        SavableDebugAddItemUIState debugAddItemUIState =
            (SavableDebugAddItemUIState)savableState;

        amountInputField.text = debugAddItemUIState.AmountInputFieldText;
        itemDropdown.value = debugAddItemUIState.ItemDropdownValue;
    }

    public string GetSaveGuid() => saveGuid;

    [Serializable]
    public class SavableDebugAddItemUIState : SavableState
    {
        public string AmountInputFieldText;
        public int ItemDropdownValue;

        public override Type GetSavableClassType() =>
            typeof(DebugAddItemUISaveManager);
    }

#if UNITY_EDITOR
    [ContextMenu("Set Save GUID")]
    private void SetSaveGuid()
    {
        saveGuid = Guid.NewGuid().ToString();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
#endif
}
