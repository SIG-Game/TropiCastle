public abstract class ContainerItemInteractable<TItemInstanceProperties> :
    ItemInteractable where TItemInstanceProperties : ContainerItemInstanceProperties
{
    protected Inventory inventory;
    protected TItemInstanceProperties itemInstanceProperties;

    protected virtual void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();

        itemInstanceProperties =
            (TItemInstanceProperties)GetComponent<ItemWorld>()
                .GetItem().instanceProperties;

        inventory.InitializeItemListWithSize(
            itemInstanceProperties.InventorySize);

        if (itemInstanceProperties != null)
        {
            inventory.SetUpFromSerializableInventory(
                itemInstanceProperties.SerializableInventory);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
    }

    protected virtual void Inventory_OnItemChangedAtIndex(ItemStack _, int _1)
    {
        itemInstanceProperties.UpdateSerializableInventory(inventory);
    }
}
