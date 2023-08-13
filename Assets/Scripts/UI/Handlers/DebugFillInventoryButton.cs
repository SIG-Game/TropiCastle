using UnityEngine;

public class DebugFillInventoryButton : MonoBehaviour
{
    [SerializeField] private PlayerInventoryUIController playerInventoryUIController;
    [SerializeField] private Inventory inventory;

    public void DebugFillInventoryButton_OnClick()
    {
        inventory.FillInventory();
    }
}
