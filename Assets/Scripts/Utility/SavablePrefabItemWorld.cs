using System;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;

public class SavablePrefabItemWorld : SavablePrefab
{
    [SerializeField] private ItemWorld itemWorld;
    [SerializeField] private Spawnable spawnable;

    public override SavablePrefabState GetSavablePrefabState()
    {
        var serializableItem = new SerializableInventoryItem(itemWorld.GetItem());

        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToString() },
            { "Item", serializableItem },
            { "SpawnerGuid", spawnable.GetSpawnerGuid() }
        };

        var savableState = new SavablePrefabState
        {
            PrefabGameObjectName = "ItemWorld",
            Properties = properties
        };

        return savableState;
    }

    public override void SetUpFromSavablePrefabState(SavablePrefabState savableState)
    {
        transform.position =
            Vector3Helper.FromString((string)savableState.Properties["Position"]);

        SerializableInventoryItem serializableItem =
            (SerializableInventoryItem)savableState.Properties["Item"];

        ItemScriptableObject itemScriptableObject =
            ItemScriptableObject.FromName(serializableItem.ItemName);

        ItemStack item = new ItemStack(itemScriptableObject,
            serializableItem.Amount,
            serializableItem.InstanceProperties);

        itemWorld.SetItem(item);

        spawnable.SetSpawnerUsingGuid<ItemSpawner>(
            (string)savableState.Properties["SpawnerGuid"]);
    }

    public override Type GetDependencySetterType() =>
        typeof(SavableItemWorldDependencySetter);

    public ItemWorld GetItemWorld() => itemWorld;
}
