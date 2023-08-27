using TMPro;
using UnityEngine;

public class DebugAddItemButton : MonoBehaviour
{
    [SerializeField] private GameObject debugAddItemButton;
    [SerializeField] private DebugAddItemDropdownController addItemDropdownController;
    [SerializeField] private TMP_InputField amountInputField;
    [SerializeField] private Inventory playerInventory;

    private void Start()
    {
        if (!DebugModeController.DebugModeEnabled)
        {
            debugAddItemButton.SetActive(false);
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
