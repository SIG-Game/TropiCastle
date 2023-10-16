public class CampfireItemInteractable : ItemInteractable
{
    private Inventory campfireInventory;
    private CampfireItemInstanceProperties campfireItemInstanceProperties;
    private CampfireUIController campfireUIController;

    private void Awake()
    {
        campfireInventory = gameObject.AddComponent<Inventory>();

        campfireItemInstanceProperties =
            (CampfireItemInstanceProperties)GetComponent<ItemWorld>()
                .GetItem().instanceProperties;

        campfireInventory.InitializeItemListWithSize(
            CampfireItemInstanceProperties.CampfireInventorySize);

        if (campfireItemInstanceProperties != null)
        {
            campfireInventory.SetUpFromSerializableInventory(
                campfireItemInstanceProperties.SerializableInventory);
        }

        campfireInventory.OnItemChangedAtIndex +=
            CampfireInventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        campfireInventory.OnItemChangedAtIndex -=
            CampfireInventory_OnItemChangedAtIndex;
    }

    public override void Interact(PlayerController _)
    {
        campfireUIController.SetInventory(campfireInventory);

        campfireUIController.Show();
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        campfireUIController =
            itemInteractableDependencies.GetCampfireUIController();
    }

    private void CampfireInventory_OnItemChangedAtIndex(ItemStack _, int _1)
    {
        campfireItemInstanceProperties.UpdateSerializableInventory(campfireInventory);
    }
}
