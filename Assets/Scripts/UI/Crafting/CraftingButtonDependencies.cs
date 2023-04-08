using UnityEngine;

public class CraftingButtonDependencies : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private Inventory playerInventory;

    public Crafting GetCrafting() => crafting;

    public Inventory GetPlayerInventory() => playerInventory;
}
