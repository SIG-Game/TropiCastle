using TMPro;
using UnityEngine;

public class DebugAddItemAmountInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private DebugAddItemDropdownController debugAddItemDropdownController;

    private void Start()
    {
        if (!DebugModeController.DebugModeEnabled)
        {
            amountInputField.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (amountInputField.isFocused)
        {
            InputManager.Instance.DisableGettingNumberKeyInputThisFrame();
        }
    }

    public void AmountInputField_OnEndEdit(string _)
    {
        ClampAmountToStackSize();
    }

    public void ClampAmountToStackSize()
    {
        int itemToAddStackSize =
            debugAddItemDropdownController.GetSelectedItemScriptableObject().stackSize;

        if (int.TryParse(amountInputField.text, out int amount) &&
            itemToAddStackSize < amount)
        {
            amountInputField.text = itemToAddStackSize.ToString();
            amountInputField.ActivateInputField();
        }
    }
}
