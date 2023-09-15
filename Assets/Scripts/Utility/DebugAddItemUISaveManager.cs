using System;
using TMPro;
using UnityEngine;

public class DebugAddItemUISaveManager : SaveManager
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;

    public override SavableState GetSavableState()
    {
        var savableState = new SavableDebugAddItemUIState
        {
            SaveGuid = saveGuid,
            AmountInputFieldText = amountInputField.text,
            ItemDropdownValue = itemDropdown.value
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
    {
        SavableDebugAddItemUIState debugAddItemUIState =
            (SavableDebugAddItemUIState)savableState;

        amountInputField.text = debugAddItemUIState.AmountInputFieldText;
        itemDropdown.value = debugAddItemUIState.ItemDropdownValue;
    }

    [Serializable]
    public class SavableDebugAddItemUIState : SavableState
    {
        public string AmountInputFieldText;
        public int ItemDropdownValue;

        public override Type GetSavableClassType() =>
            typeof(DebugAddItemUISaveManager);
    }
}
