using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private ItemWithAmount item;
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
    }

    public void UpdateItemInstanceProperties()
    {
        if (item.itemData.name == "Chest")
        {
            item.instanceProperties = new ChestItemInstanceProperties
            {
                SerializableInventory = GetComponent<Inventory>().GetSerializableInventory()
            };
        }
    }
}
