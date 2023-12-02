using System.Collections.Generic;
using UnityEngine;

public class InventorySaveManager : SaveManager
{
    [SerializeField] private Inventory inventory;

    public override SaveManagerState GetState()
    {
        List<SerializableItem> serializableItemList =
            inventory.GetAsSerializableItemList();

        var properties = new Dictionary<string, object>
        {
            { "ItemList", serializableItemList }
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = properties
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        inventory.SetUpFromSerializableItemList((List<SerializableItem>)
            saveManagerState.Properties["ItemList"]);
    }
}
