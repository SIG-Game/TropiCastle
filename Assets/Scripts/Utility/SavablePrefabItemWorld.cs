using System.Collections.Generic;
using UnityEngine;

public class SavablePrefabItemWorld : SavablePrefab
{
    [SerializeField] private ItemWorld itemWorld;
    [SerializeField] private Spawnable spawnable;

    public override string PrefabGameObjectName => "ItemWorld";

    public override Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>
        {
            { "Position", transform.position.ToArray() },
            { "Item", itemWorld.Item },
            { "SpawnerGuid", spawnable.GetSpawnerGuid() }
        };

        return properties;
    }

    public override void SetUpFromProperties(Dictionary<string, object> properties)
    {
        transform.position = Vector3Helper.FromArray((float[])properties["Position"]);

        itemWorld.Item = (ItemStack)properties["Item"];

        spawnable.SetSpawnerUsingGuid<ItemSpawner>((string)properties["SpawnerGuid"]);
    }
}
