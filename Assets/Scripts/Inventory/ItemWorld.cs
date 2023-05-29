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

    public ItemWithAmount Item
    {
        get => item;
        set
        {
            item = value;
        }
    }

    private static readonly Dictionary<string, Type> itemNameToInteractableType =
        new Dictionary<string, Type>
    {
        { "Campfire", typeof(CampfireItemInteractable) },
        { "Chest", typeof(ChestItemInteractable) }
    };

    // These operations must be in the Start method because the Awake
    // method runs before ItemWorldPrefabInstanceFactory sets item
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemData.sprite;

        if (item.itemData.hasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = item.itemData.customColliderSize;
        }

        if (itemNameToInteractableType.TryGetValue(item.itemData.name,
            out Type itemInteractableType))
        {
            gameObject.AddComponent(itemInteractableType);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{item.itemData.name} ItemWorld";

        amountText.text = item.GetAmountText();
    }

    public SerializableItemWorldState GetSerializableState()
    {
        var serializableItem = new SerializableInventoryItem
        {
            // Use ScriptableObject name and not item display name
            ItemName = ((ScriptableObject)item.itemData).name,
            Amount = item.amount,
            InstanceProperties = item.instanceProperties
        };

        var serializableState = new SerializableItemWorldState
        {
            Item = serializableItem,
            Position = transform.position,
            GameObjectName = gameObject.name
        };

        return serializableState;
    }

    [Serializable]
    public class SerializableItemWorldState
    {
        public SerializableInventoryItem Item;
        public Vector2 Position;
        public string GameObjectName;
    }
}
