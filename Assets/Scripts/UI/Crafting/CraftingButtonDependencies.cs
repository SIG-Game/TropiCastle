using UnityEngine;

public class CraftingButtonDependencies : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;

    public Inventory PlayerInventory => playerInventory;

    public InventoryUIHeldItemController InventoryUIHeldItemController =>
        inventoryUIHeldItemController;
}
