using TMPro;
using UnityEngine;

public class DebugAddItemAmountInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private InputManager inputManager;

    private void Start()
    {
        if (!DebugController.DebugModeEnabled)
        {
            amountInputField.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (amountInputField.isFocused)
        {
            inputManager.DisableGettingNumberKeyInputThisFrame();
        }
    }
}
