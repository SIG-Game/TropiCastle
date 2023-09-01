using UnityEngine;

public class CraftingButtonDependencies : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;

    public Inventory GetPlayerInventory() => playerInventory;

    public InventoryUIHeldItemController GetInventoryUIHeldItemController() =>
        inventoryUIHeldItemController;
}
