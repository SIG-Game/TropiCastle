using System;
using System.Collections.Generic;
using UnityEngine;

public class SavablePrefabItemWorld : SavablePrefab
{
    [SerializeField] private ItemWorld itemWorld;
    [SerializeField] private Spawnable spawnable;

    public override SavablePrefabState GetSavablePrefabState()
    {
        var serializableItem = new SerializableItem(itemWorld.Item);

        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToArray() },
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
            Vector3Helper.FromArray((float[])savableState.Properties["Position"]);

        SerializableItem serializableItem =
            (SerializableItem)savableState.Properties["Item"];

        ItemScriptableObject itemScriptableObject =
            ItemScriptableObject.FromName(serializableItem.ItemName);

        itemWorld.Item = new ItemStack(itemScriptableObject,
            serializableItem.Amount,
            serializableItem.InstanceProperties);

        spawnable.SetSpawnerUsingGuid<ItemSpawner>(
            (string)savableState.Properties["SpawnerGuid"]);
    }

    public override Type GetDependencySetterType() =>
        typeof(SavableItemWorldDependencySetter);

    public ItemWorld GetItemWorld() => itemWorld;
}
