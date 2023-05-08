using UnityEngine;

public class DebugFillInventoryButton : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;

    public void DebugFillInventoryButton_OnClick()
    {
        playerInventory.FillInventory();
    }
}
