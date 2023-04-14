using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : Interactable
{
    [SerializeField] public ItemWithAmount item;

    private IItemInteraction itemInteraction;

    private static Dictionary<string, IItemInteraction> itemNameToInteraction =
        new Dictionary<string, IItemInteraction>
    {
        { "Campfire", new CampfireItemInteraction() }
    };

    // These operations must be in the Start method because the Awake
    // method runs before ItemWorldPrefabInstanceFactory sets item
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemData.sprite;

        if (item.itemData.hasCustomColliderSize) {
            GetComponent<BoxCollider2D>().size = item.itemData.customColliderSize;
        }

        itemInteraction = itemNameToInteraction.GetValueOrDefault(item.itemData.name);

        name = $"{item.itemData.name} ItemWorld";
    }

    public override void Interact(PlayerController player)
    {
        if (itemInteraction != null)
        {
            Debug.Log("Item interaction with item interaction type " + itemInteraction.GetType().Name);
            itemInteraction.Interact(player);
        }
    }
}
