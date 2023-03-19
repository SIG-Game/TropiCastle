using UnityEngine;

public class DebugAddItemButton : MonoBehaviour
{
    [SerializeField] private DebugAddItemDropdownController addItemDropdownController;
    [SerializeField] private Inventory playerInventory;

    public void DebugAddItemButton_OnClick()
    {
        playerInventory.AddItem(addItemDropdownController.GetSelectedItemScriptableObject(), 1);
    }
}
