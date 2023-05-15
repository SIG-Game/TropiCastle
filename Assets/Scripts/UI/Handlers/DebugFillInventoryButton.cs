using UnityEngine;

public class DebugFillInventoryButton : MonoBehaviour
{
    [SerializeField] private InventoryUIController inventoryUIController;

    private Inventory playerInventory;

    private void Awake()
    {
        playerInventory = inventoryUIController.GetInventory();
    }

    public void DebugFillInventoryButton_OnClick()
    {
        playerInventory.FillInventory();
    }
}
