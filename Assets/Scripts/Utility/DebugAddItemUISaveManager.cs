using System;
using TMPro;
using UnityEngine;

public class DebugAddItemUISaveManager : MonoBehaviour,
    ISavable<DebugAddItemUISaveManager.SerializableDebugAddItemUIState>
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private TMP_Dropdown itemDropdown;

    public SerializableDebugAddItemUIState GetSerializableState()
    {
        var serializableState = new SerializableDebugAddItemUIState
        {
            AmountInputFieldText = amountInputField.text,
            ItemDropdownValue = itemDropdown.value
        };

        return serializableState;
    }

    public void SetPropertiesFromSerializableState(
        SerializableDebugAddItemUIState serializableState)
    {
        amountInputField.text = serializableState.AmountInputFieldText;
        itemDropdown.value = serializableState.ItemDropdownValue;
    }

    [Serializable]
    public class SerializableDebugAddItemUIState
    {
        public string AmountInputFieldText;
        public int ItemDropdownValue;
    }
}
