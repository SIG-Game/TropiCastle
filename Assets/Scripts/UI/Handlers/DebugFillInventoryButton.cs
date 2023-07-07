using UnityEngine;

public class DebugFillInventoryButton : MonoBehaviour
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;

    private Inventory playerInventory;

    private void Awake()
    {
        playerInventory = playerInventoryUIController.GetInventory();
    }

    public void DebugFillInventoryButton_OnClick()
    {
        playerInventory.FillInventory();
    }
}
