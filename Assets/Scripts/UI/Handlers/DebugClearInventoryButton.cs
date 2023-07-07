using UnityEngine;

public class DebugClearInventoryButton : MonoBehaviour
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;

    private Inventory playerInventory;

    private void Awake()
    {
        playerInventory = playerInventoryUIController.GetInventory();
    }

    public void DebugClearInventoryButton_OnClick()
    {
        playerInventory.ClearInventory();
    }
}
