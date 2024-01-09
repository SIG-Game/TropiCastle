using TMPro;
using UnityEngine;

public class DebugAddItemAmountInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField amountInputField;
    
    [Inject] private InputManager inputManager;

    private void Awake()
    {
        this.InjectDependencies();
    }

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
