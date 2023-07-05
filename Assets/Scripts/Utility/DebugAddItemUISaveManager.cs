using System;
using TMPro;
using UnityEngine;

public class DebugAddItemUISaveManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;

    public SerializableDebugAddItemUIState GetSerializableDebugAddItemUIState()
    {
        var debugAddItemUIState = new SerializableDebugAddItemUIState
        {
            AmountInputFieldText = amountInputField.text,
            ItemDropdownValue = itemDropdown.value
        };

        return debugAddItemUIState;
    }

    public void UpdateDebugAddItemUIUsingState(
        SerializableDebugAddItemUIState debugAddItemUIState)
    {
        amountInputField.text = debugAddItemUIState.AmountInputFieldText;
        itemDropdown.value = debugAddItemUIState.ItemDropdownValue;
    }

    [Serializable]
    public class SerializableDebugAddItemUIState
    {
        public string AmountInputFieldText;
        public int ItemDropdownValue;
    }
}
