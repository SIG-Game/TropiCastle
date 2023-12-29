using System.Collections.Generic;

public abstract class ContainerItemInteractable : Interactable
{
    protected Inventory inventory;
    protected ItemInstanceProperties itemInstanceProperties;

    protected virtual void Awake()
    {
        inventory = gameObject.AddComponent<Inventory>();

        ItemStack item = GetComponent<ItemWorld>().Item;

        itemInstanceProperties = item.instanceProperties;

        inventory.InitializeItemListWithSize(
            item.itemDefinition.GetIntProperty("ContainerSize"));

        if (itemInstanceProperties != null)
        {
            var itemList = (List<ItemStackStruct>)
                itemInstanceProperties.PropertyDictionary["ItemList"];

            inventory.SetUpFromItemList(itemList);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
    }

    protected virtual void Inventory_OnItemChangedAtIndex(ItemStackStruct _, int _1)
    {
        itemInstanceProperties.UpdateItemListProperty(inventory);
    }
}
