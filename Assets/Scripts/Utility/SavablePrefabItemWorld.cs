using System;
using UnityEngine;
using static Inventory;

public class SavablePrefabItemWorld : SavablePrefab
{
    [SerializeField] private ItemWorld itemWorld;
    [SerializeField] private Spawnable spawnable;

    public override SavablePrefabState GetSavablePrefabState()
    {
        var serializableItem = new SerializableInventoryItem(itemWorld.GetItem());

        var savableState = new SavableItemWorldState
        {
            Item = serializableItem,
            Position = transform.position,
            SpawnerGuid = spawnable.GetSpawnerGuid()
        };

        return savableState;
    }

    public override void SetUpFromSavablePrefabState(SavablePrefabState savableState)
    {
        SavableItemWorldState itemWorldState = (SavableItemWorldState)savableState;

        transform.position = itemWorldState.Position;

        ItemScriptableObject itemScriptableObject =
            ItemScriptableObject.FromName(itemWorldState.Item.ItemName);

        ItemStack item = new ItemStack(itemScriptableObject,
            itemWorldState.Item.Amount,
            itemWorldState.Item.InstanceProperties);

        itemWorld.SetItem(item);

        spawnable
            .SetSpawnerUsingGuid<ItemSpawner>(itemWorldState.SpawnerGuid);
    }

    public override Type GetDependencySetterType() =>
        typeof(SavableItemWorldDependencySetter);

    public ItemWorld GetItemWorld() => itemWorld;

    [Serializable]
    public class SavableItemWorldState : SavablePrefabState
    {
        public SerializableInventoryItem Item;
        public Vector2 Position;
        public string SpawnerGuid;

        public override string GetPrefabGameObjectName() => "ItemWorld";
    }
}
