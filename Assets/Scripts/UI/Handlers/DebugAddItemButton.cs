using TMPro;
using UnityEngine;

public class DebugAddItemButton : MonoBehaviour
{
    [SerializeField] private GameObject debugAddItemButton;
    [SerializeField] private DebugAddItemDropdownController addItemDropdownController;
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemSelectionController itemSelectionController;

    private void Start()
    {
        if (!DebugModeController.DebugModeEnabled)
        {
            debugAddItemButton.SetActive(false);
            amountInputField.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (amountInputField.isFocused)
        {
            InputManager.Instance.NumberKeyUsedThisFrame = true;
        }
    }

    public void DebugAddItemButton_OnClick()
    {
        ItemScriptableObject selectedItemScriptableObject =
            addItemDropdownController.GetSelectedItemScriptableObject();

        if (int.TryParse(amountInputField.text, out int amount))
        {
            playerInventory.AddItem(selectedItemScriptableObject, amount);
        }
        else
        {
            playerInventory.AddItem(selectedItemScriptableObject, 1);
        }
    }
}
