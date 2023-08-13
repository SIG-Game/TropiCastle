using UnityEngine;

public class CraftingButtonDependencies : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;

    public Crafting GetCrafting() => crafting;

    public Inventory GetPlayerInventory() => playerInventory;

    public InventoryUIHeldItemController GetInventoryUIHeldItemController() =>
        inventoryUIHeldItemController;
}
