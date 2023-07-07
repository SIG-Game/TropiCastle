public class ChestItemInteractable : Interactable
{
    private Inventory chestInventory;
    private ItemWorld chestItemWorld;
    private ChestItemInstanceProperties chestItemInstanceProperties;

    private void Awake()
    {
        chestInventory = gameObject.AddComponent<Inventory>();
        chestItemWorld = GetComponent<ItemWorld>();
        chestItemInstanceProperties =
            (ChestItemInstanceProperties)chestItemWorld.Item.instanceProperties;

        chestInventory.InitializeItemListWithSize(
            ChestItemInstanceProperties.ChestInventorySize);

        if (chestItemInstanceProperties != null)
        {
            chestInventory.SetInventoryFromSerializableInventory(
                chestItemInstanceProperties.SerializableInventory);
        }

        chestInventory.OnItemChangedAtIndex += ChestInventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        chestInventory.OnItemChangedAtIndex -= ChestInventory_OnItemChangedAtIndex;
    }

    private void ChestInventory_OnItemChangedAtIndex(ItemWithAmount _, int _1)
    {
        chestItemInstanceProperties.UpdateSerializableInventory(chestInventory);
    }

    public override void Interact(PlayerController _)
    {
        ChestUIController.Instance.SetChestInventory(chestInventory);

        ChestUIController.Instance.ShowChestUI();
    }
}
