using UnityEngine;

public class DebugAddItemButton : MonoBehaviour
{
    [SerializeField] private DebugAddItemDropdownController addItemDropdownController;
    [SerializeField] private PlayerController player;

    private Inventory playerInventory;

    private void Start()
    {
        playerInventory = player.GetInventory();
    }

    public void DebugAddItemButton_OnClick()
    {
        playerInventory.AddItem(addItemDropdownController.GetSelectedItemScriptableObject(), 1);
    }
}
