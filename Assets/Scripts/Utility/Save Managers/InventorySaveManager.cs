using System.Collections.Generic;
using UnityEngine;

public class InventorySaveManager : SaveManager
{
    [SerializeField] private Inventory inventory;

    public override Dictionary<string, object> GetProperties()
    {
        List<SerializableItem> serializableItemList =
            inventory.GetAsSerializableItemList();

        var properties = new Dictionary<string, object>
        {
            { "ItemList", serializableItemList }
        };

        return properties;
    }

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        inventory.SetUpFromSerializableItemList(
            (List<SerializableItem>)properties["ItemList"]);
    }
}
