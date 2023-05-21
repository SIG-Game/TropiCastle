using UnityEngine;

public class DebugClearInventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryUIController inventoryUIController;

    private Inventory playerInventory;

    private void Awake()
    {
        playerInventory = inventoryUIController.GetInventory();
    }

    public void DebugClearInventoryButton_OnClick()
    {
        playerInventory.ClearInventory();
    }
}
