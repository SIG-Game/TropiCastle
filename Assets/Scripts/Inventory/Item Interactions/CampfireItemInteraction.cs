using System;
using UnityEngine;

public class CampfireItemInteraction : IItemInteraction
{
    private readonly Lazy<ItemScriptableObject> lazyCookedCrabMeat;

    public CampfireItemInteraction()
    {
        lazyCookedCrabMeat =
            new Lazy<ItemScriptableObject>(LoadCookedCrabMeatItemScriptableObject);
    }

    public void Interact(PlayerController playerController)
    {
        Inventory playerInventory = playerController.GetInventory();

        int rawCrabMeatIndex = playerInventory.GetItemList()
            .FindIndex(x => x.itemData.name == "Raw Crab Meat");

        if (rawCrabMeatIndex != -1)
        {
            playerInventory.RemoveItemAtIndex(rawCrabMeatIndex);
            playerInventory.AddItem(lazyCookedCrabMeat.Value, 1);
        }
    }

    private ItemScriptableObject LoadCookedCrabMeatItemScriptableObject() =>
        Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat");
}
