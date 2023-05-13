public class ChestItemInteractable : Interactable
{
    private Inventory inventory;
    private ItemWorld itemWorld;


    private void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();
        itemWorld = GetComponent<ItemWorld>();

        inventory.InitializeItemListWithSize(ChestItemInstanceProperties.ChestInventorySize);

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;

        if (itemWorld.Item.instanceProperties != null)
        {
            inventory.SetInventoryFromSerializableInventory(
                ((ChestItemInstanceProperties)itemWorld.Item
                .instanceProperties).SerializableInventory);
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
        }
    }

    private void Inventory_OnItemChangedAtIndex(ItemWithAmount _, int _1)
    {
        ((ChestItemInstanceProperties)itemWorld.Item
            .instanceProperties).UpdateSerializableInventory(inventory);
    }

    public override void Interact(PlayerController playerController)
    {
        ChestUIController.Instance.SetChestInventory(inventory);
        ChestUIController.Instance.SetPlayerInventory(playerController.GetInventory());

        ChestUIController.Instance.ShowChestUI();
    }
}
