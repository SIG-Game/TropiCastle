using UnityEngine;

public class CampfireItemInteraction : IItemInteraction
{
    public void Interact(PlayerController playerController)
    {
        Inventory playerInventory = playerController.GetInventory();

        int rawCrabMeatIndex = playerInventory.GetItemList()
            .FindIndex(x => x.itemData.name == "Raw Crab Meat");

        if (rawCrabMeatIndex != -1)
        {
            playerInventory.RemoveItemAtIndex(rawCrabMeatIndex);
            playerInventory.AddItem(Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat"), 1);
        }
    }
}
