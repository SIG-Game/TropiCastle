using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugAddItemUISaveManager : SaveManager
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;

    public override SaveManagerState GetState()
    {
        var propertyList = new List<Property>()
        {
            new Property("AmountInputFieldText", amountInputField.text),
            new Property("ItemDropdownValue", itemDropdown.value.ToString())
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = new PropertyCollection(propertyList)
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        amountInputField.text = saveManagerState.Properties
            .GetStringProperty("AmountInputFieldText");
        itemDropdown.value = saveManagerState.Properties
            .GetIntProperty("ItemDropdownValue");
    }
}
