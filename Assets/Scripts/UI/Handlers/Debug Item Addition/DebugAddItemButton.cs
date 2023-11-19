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
        if (!DebugController.DebugModeEnabled)
        {
            debugAddItemButton.SetActive(false);
        }
    }

    public void DebugAddItemButton_OnClick()
    {
        ItemScriptableObject selectedItemScriptableObject =
            addItemDropdownController.GetSelectedItemScriptableObject();

        if (!int.TryParse(amountInputField.text, out int amountToAdd) ||
            amountToAdd <= 0)
        {
            amountToAdd = 1;
        }

        int stackSize = selectedItemScriptableObject.StackSize;
        int numberOfStacks = amountToAdd / stackSize;
        int remainingAmount = amountToAdd % stackSize;

        for (int i = 0; i < numberOfStacks; ++i)
        {
            playerInventory.AddItem(selectedItemScriptableObject, stackSize);
        }

        if (remainingAmount != 0)
        {
            playerInventory.AddItem(selectedItemScriptableObject, remainingAmount);
        }
    }
}
