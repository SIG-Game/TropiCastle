using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private ItemWithAmount item;
    [SerializeField] private bool logOnInteract;

    private static readonly Dictionary<string, Type> itemNameToInteractableType =
        new Dictionary<string, Type>
    {
        { "Campfire", typeof(CampfireItemInteractable) }
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

    public ItemWithAmount GetItem() => item;

    public void SetItem(ItemWithAmount item)
    {
        this.item = item;
    }
}
