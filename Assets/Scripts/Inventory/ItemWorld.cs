using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Inventory;

public class ItemWorld : MonoBehaviour
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

    private Spawnable spawnable;

    private void Awake()
    {
        spawnable = GetComponent<Spawnable>();
    }

    public void SetUpItemWorld(ItemWithAmount item,
        ItemInteractableDependencies itemInteractableDependencies)
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

    public void SetItemAmount(int amount)
    {
        item.amount = amount;

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

    [Serializable]
    public class SerializableItemWorldState
    {
        public SerializableInventoryItem Item;
        public Vector2 Position;
        public string GameObjectName;
        public int SpawnerId;
    }
}
