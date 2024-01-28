using System;
using TMPro;
using UnityEngine;

public class DebugAddItemButton : MonoBehaviour
{
    [SerializeField] private GameObject debugAddItemButton;
    [SerializeField] private DebugAddItemDropdownController addItemDropdownController;
    [SerializeField] private TMP_InputField amountInputField;

    [Inject("PlayerInventory")] private Inventory playerInventory;

    private void Awake()
    {
        this.InjectDependencies();
    }
    
    private void Start()
    {
        if (!DebugController.DebugModeEnabled)
        {
            debugAddItemButton.SetActive(false);
        }
    }

    public void DebugAddItemButton_OnClick()
    {
        ItemScriptableObject selectedItemDefinition =
            addItemDropdownController.GetSelectedItemDefinition();

        if (!int.TryParse(amountInputField.text, out int addAmount) ||
            addAmount <= 0)
        {
            addAmount = 1;
        }

        while (addAmount > 0)
        {
            int iterationAddAmount =
                Math.Min(addAmount, selectedItemDefinition.StackSize);

            playerInventory.AddItem(selectedItemDefinition, iterationAddAmount);

            addAmount -= iterationAddAmount;
        }
    }
}
