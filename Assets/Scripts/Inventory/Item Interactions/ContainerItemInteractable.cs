using System.Collections.Generic;

public abstract class ContainerItemInteractable : Interactable
{
    protected Inventory inventory;
    protected ItemInstanceProperties itemInstanceProperties;

    protected virtual void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();

        ItemStack item = GetComponent<ItemWorld>().Item;

        itemInstanceProperties = item.InstanceProperties;

        inventory.InitializeItemListWithSize(
            item.ItemDefinition.GetIntProperty("ContainerSize"));

        if (itemInstanceProperties != null)
        {
            var itemList = (List<ItemStack>)
                itemInstanceProperties.PropertyDictionary["ItemList"];

            inventory.SetUpFromItemList(itemList);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
    }

    protected virtual void Inventory_OnItemChangedAtIndex(ItemStack _, int _1)
    {
        itemInstanceProperties.UpdateItemListProperty(inventory);
    }
}
