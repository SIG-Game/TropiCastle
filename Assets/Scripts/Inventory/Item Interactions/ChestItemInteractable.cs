public class ChestItemInteractable : ItemInteractable
{
    private Inventory chestInventory;
    private ItemWorld chestItemWorld;
    private ChestItemInstanceProperties chestItemInstanceProperties;
    private ChestUIController chestUIController;

    private void Awake()
    {
        chestInventory = gameObject.AddComponent<Inventory>();
        chestItemWorld = GetComponent<ItemWorld>();
        chestItemInstanceProperties =
            (ChestItemInstanceProperties)chestItemWorld.GetItem().instanceProperties;

        chestInventory.InitializeItemListWithSize(
            ChestItemInstanceProperties.ChestInventorySize);

        if (chestItemInstanceProperties != null)
        {
            chestInventory.SetPropertiesFromSerializableState(
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
        chestUIController.SetChestInventory(chestInventory);

        chestUIController.ShowChestUI();
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        chestUIController =
            itemInteractableDependencies.GetChestUIController();
    }
}
