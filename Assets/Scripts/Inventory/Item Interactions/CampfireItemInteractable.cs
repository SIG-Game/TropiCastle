using System;
using UnityEngine;

public class CampfireItemInteractable : Interactable
{
    private static readonly Lazy<ItemScriptableObject> lazyCookedCrabMeat =
        new Lazy<ItemScriptableObject>(LoadCookedCrabMeatItemScriptableObject);

    public override void Interact(PlayerController playerController)
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

    private static ItemScriptableObject LoadCookedCrabMeatItemScriptableObject() =>
        Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat");
}
