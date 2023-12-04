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

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        amountInputField.text = (string)properties["AmountInputFieldText"];
        itemDropdown.value = Convert.ToInt32(properties["ItemDropdownValue"]);
    }
}
