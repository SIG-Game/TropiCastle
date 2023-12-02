using System.Collections.Generic;

public abstract class ContainerItemInteractable : ItemInteractable
{
    protected Inventory inventory;
    protected ContainerItemInstanceProperties itemInstanceProperties;

    protected virtual void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();

        ItemStack item = GetComponent<ItemWorld>().Item;

        itemInstanceProperties =
            (ContainerItemInstanceProperties)item.instanceProperties;

        inventory.InitializeItemListWithSize(
            item.itemDefinition.GetIntProperty("ContainerSize"));

        if (itemInstanceProperties != null)
        {
            var serializableItemList = (List<SerializableItem>)
                itemInstanceProperties.PropertyDictionary["ItemList"];

            inventory.SetUpFromSerializableItemList(serializableItemList);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
    }

    protected virtual void Inventory_OnItemChangedAtIndex(ItemStack _, int _1)
    {
        itemInstanceProperties.UpdateSerializableItemList(inventory);
    }
}
