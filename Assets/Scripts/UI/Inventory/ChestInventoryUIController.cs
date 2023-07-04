using System.Collections.Generic;
using UnityEngine;

public class ChestInventoryUIController : ItemSlotContainerController
{
    [SerializeField] private HoveredItemSlotManager hoveredItemSlotManager;

    protected override void Awake()
    {
        // Skip subscribing to inventory.OnItemChangedAtIndex event in base class Awake method
    }

    protected override void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
        }
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        List<ItemWithAmount> itemList = inventory.GetItemList();

        for (int i = 0; i < itemSlotControllers.Count; ++i)
        {
            InventoryUIItemSlotController itemSlotController =
                itemSlotControllers[i] as InventoryUIItemSlotController;
            ItemWithAmount item = itemList[i];

            itemSlotController.SetInventory(inventory);
            itemSlotController.UpdateUsingItem(item);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }
}
