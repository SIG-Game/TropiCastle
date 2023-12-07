using System;
using System.Collections.Generic;
using UnityEngine;

public class SavablePrefabItemWorld : SavablePrefab
{
    [SerializeField] private ItemWorld itemWorld;
    [SerializeField] private Spawnable spawnable;

    public override string PrefabGameObjectName => "ItemWorld";

    public override Dictionary<string, object> GetProperties()
    {
        var serializableItem = new SerializableItem(itemWorld.Item);

        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToArray() },
            { "Item", serializableItem },
            { "SpawnerGuid", spawnable.GetSpawnerGuid() }
        };

        return properties;
    }

    public override void SetUpFromProperties(Dictionary<string, object> properties)
    {
        transform.position = Vector3Helper.FromArray((float[])properties["Position"]);

        var serializableItem = (SerializableItem)properties["Item"];

        var itemScriptableObject =
            ItemScriptableObject.FromName(serializableItem.ItemName);

        itemWorld.Item = new ItemStack(itemScriptableObject,
            serializableItem.Amount,
            serializableItem.InstanceProperties);

        spawnable.SetSpawnerUsingGuid<ItemSpawner>((string)properties["SpawnerGuid"]);
    }

    public override Type GetDependencySetterType() =>
        typeof(SavableItemWorldDependencySetter);

    public ItemWorld GetItemWorld() => itemWorld;
}
