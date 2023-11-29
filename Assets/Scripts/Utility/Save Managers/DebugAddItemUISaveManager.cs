using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugAddItemUISaveManager : SaveManager
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;

    public override SaveManagerState GetState()
    {
        var properties = new Dictionary<string, object>
        {
            { "AmountInputFieldText", amountInputField.text },
            { "ItemDropdownValue", itemDropdown.value }
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = properties
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        amountInputField.text =
            (string)saveManagerState.Properties["AmountInputFieldText"];
        itemDropdown.value =
           Convert.ToInt32(saveManagerState.Properties["ItemDropdownValue"]);
    }
}
