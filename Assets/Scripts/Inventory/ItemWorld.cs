using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Inventory;

public class ItemWorld : MonoBehaviour, ISavable<ItemWorld.SerializableItemWorldState>
{
    [SerializeField] private ItemWithAmount item;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private bool logOnInteract;

    private static readonly Dictionary<string, Type> itemNameToInteractableType =
        new Dictionary<string, Type>
    {
        { "Campfire", typeof(CampfireItemInteractable) },
        { "Chest", typeof(ChestItemInteractable) }
    };

    private ItemInteractableDependencies itemInteractableDependencies;
    private Spawnable spawnable;

    private void Awake()
    {
        spawnable = GetComponent<Spawnable>();
    }

    public void SetUpItemWorld(ItemWithAmount item,
        ItemInteractableDependencies itemInteractableDependencies)
    {
        SetItemInteractableDependencies(itemInteractableDependencies);
        SetItem(item);
    }

    public void SetItemAmount(int amount)
    {
        item.amount = amount;

        amountText.text = item.GetAmountText();
    }

    public void SetItemInteractableDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        this.itemInteractableDependencies = itemInteractableDependencies;
    }

    private void SetItem(ItemWithAmount item)
    {
        this.item = item;

        GetComponent<SpriteRenderer>().sprite = item.itemData.sprite;

        if (item.itemData.hasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = item.itemData.customColliderSize;
        }

        if (itemNameToInteractableType.TryGetValue(item.itemData.name,
            out Type itemInteractableType))
        {
            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{item.itemData.name} ItemWorld";

        amountText.text = item.GetAmountText();
    }

    public ItemWithAmount GetItem() => item;

    public SerializableItemWorldState GetSerializableState()
    {
        var serializableItem = new SerializableInventoryItem(item);

        var serializableState = new SerializableItemWorldState
        {
            Item = serializableItem,
            Position = transform.position,
            GameObjectName = gameObject.name,
            SpawnerId = spawnable.GetSpawnerId()
        };

        return serializableState;
    }

    public void SetPropertiesFromSerializableState(
        SerializableItemWorldState serializableState)
    {
        transform.position = serializableState.Position;

        ItemScriptableObject itemScriptableObject =
            Resources.Load<ItemScriptableObject>($"Items/{serializableState.Item.ItemName}");

        ItemWithAmount item = new ItemWithAmount(itemScriptableObject,
            serializableState.Item.Amount,
            serializableState.Item.InstanceProperties);

        SetItem(item);

        gameObject.name = serializableState.GameObjectName;

        GetComponent<Spawnable>()
            .SetSpawnerUsingId<ItemSpawner>(serializableState.SpawnerId);
    }

    [Serializable]
    public class SerializableItemWorldState
    {
        public SerializableInventoryItem Item;
        public Vector2 Position;
        public string GameObjectName;
        public int SpawnerId;
    }
}
