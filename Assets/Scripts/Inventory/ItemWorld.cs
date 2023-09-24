using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public void SetItem(ItemWithAmount item)
    {
        this.item = item;

        GetComponent<SpriteRenderer>().sprite = item.itemDefinition.sprite;

        if (item.itemDefinition.hasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = item.itemDefinition.customColliderSize;
        }

        if (itemNameToInteractableType.TryGetValue(item.itemDefinition.name,
            out Type itemInteractableType))
        {
            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{item.itemDefinition.name} ItemWorld";

        amountText.text = item.GetAmountText();
    }

    public ItemWithAmount GetItem() => item;
}
