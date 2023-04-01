using UnityEngine;

public class CampfireItemInteraction : IItemInteraction {
    public void Interact(PlayerController playerController)
    {
        ItemWithAmount hotbarItem = playerController.GetSelectedItem();

        if (hotbarItem.itemData.name == "Raw Crab Meat")
        {
            Inventory playerInventory = playerController.GetInventory();
            int selectedItemIndex = playerController.GetSelectedItemIndex();

            playerInventory.RemoveItemAtIndex(selectedItemIndex);
            playerInventory.AddItem(Resources.Load<ItemScriptableObject>("Items/CookedCrabMeat"), 1);
        }
    }
}
