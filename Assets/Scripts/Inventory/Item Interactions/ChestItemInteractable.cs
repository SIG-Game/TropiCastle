public class ChestItemInteractable : Interactable
{
    private Inventory inventory;

    private const int chestInventorySize = 10;

    private void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();

        inventory.InitializeItemListWithSize(chestInventorySize);

        ItemWorld itemWorld = GetComponent<ItemWorld>();

        if (itemWorld.Item.instanceProperties != null)
        {
            inventory.SetInventoryFromSerializableInventory(
                ((ChestItemInstanceProperties)itemWorld.Item
                .instanceProperties).SerializableInventory);
        }
    }

    public override void Interact(PlayerController playerController)
    {
        ChestUIController.Instance.SetChestInventory(inventory);
        ChestUIController.Instance.SetPlayerInventory(playerController.GetInventory());

        ChestUIController.Instance.ShowChestUI();
    }
}
