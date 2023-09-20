using System;
using TMPro;
using UnityEngine;

public class DebugAddItemUISaveManager : SaveManager
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;

    public override SaveManagerState GetState()
    {
        var saveManagerState = new DebugAddItemUISaveManagerState
        {
            SaveGuid = saveGuid,
            AmountInputFieldText = amountInputField.text,
            ItemDropdownValue = itemDropdown.value
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        DebugAddItemUISaveManagerState debugAddItemUIState =
            (DebugAddItemUISaveManagerState)saveManagerState;

        amountInputField.text = debugAddItemUIState.AmountInputFieldText;
        itemDropdown.value = debugAddItemUIState.ItemDropdownValue;
    }

    [Serializable]
    public class DebugAddItemUISaveManagerState : SaveManagerState
    {
        public string AmountInputFieldText;
        public int ItemDropdownValue;
    }
}
