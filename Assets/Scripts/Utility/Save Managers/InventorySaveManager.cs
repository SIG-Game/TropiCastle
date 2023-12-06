using System.Collections.Generic;
using UnityEngine;

public class InventorySaveManager : SaveManager
{
    [SerializeField] private Inventory inventory;

    public override Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>
        {
            { "ItemList", inventory.GetItemList() }
        };

        return properties;
    }

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        inventory.SetUpFromItemList((List<ItemStack>)properties["ItemList"]);
    }
}
