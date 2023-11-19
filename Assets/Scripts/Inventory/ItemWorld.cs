using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private ItemStack item;
    [SerializeField] private TMP_Text amountText;

    private static readonly Dictionary<string, Type> itemNameToInteractableType =
        new Dictionary<string, Type>
    {
        { "Campfire", typeof(CampfireItemInteractable) },
        { "Chest", typeof(ChestItemInteractable) }
    };

    private ItemInteractableDependencies itemInteractableDependencies;

    public void SetUpItemWorld(ItemStack item,
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

    public void SetItem(ItemStack item)
    {
        this.item = item;

        GetComponent<SpriteRenderer>().sprite = item.itemDefinition.Sprite;

        if (item.itemDefinition.HasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = item.itemDefinition.CustomColliderSize;
        }

        if (item.itemDefinition.TriggerCollisionPickup)
        {
            GetComponent<BoxCollider2D>().isTrigger =
                item.itemDefinition.TriggerCollisionPickup;
        }

        if (itemNameToInteractableType.TryGetValue(item.itemDefinition.Name,
            out Type itemInteractableType))
        {
            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{item.itemDefinition.Name} ItemWorld";

        amountText.text = item.GetAmountText();
    }

    public ItemStack GetItem() => item;
}
