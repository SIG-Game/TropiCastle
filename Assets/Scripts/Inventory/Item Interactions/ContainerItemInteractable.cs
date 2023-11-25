public abstract class ContainerItemInteractable : ItemInteractable
{
    protected Inventory inventory;
    protected ContainerItemInstanceProperties itemInstanceProperties;

    protected virtual void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();

        ItemStack item = GetComponent<ItemWorld>().GetItem();

        itemInstanceProperties =
            (ContainerItemInstanceProperties)item.instanceProperties;

        inventory.InitializeItemListWithSize(
            item.itemDefinition.GetIntProperty("ContainerSize"));

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
