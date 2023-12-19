using System;
using TMPro;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private ItemStackStruct item;
    [SerializeField] private TMP_Text amountText;

    public ItemStackStruct Item
    {
        get => item;
        set => SetItem(value);
    }

    private ItemInteractableDependencies itemInteractableDependencies;

    public void SetUpItemWorld(ItemStack item,
        ItemInteractableDependencies itemInteractableDependencies)
    {
        SetItemInteractableDependencies(itemInteractableDependencies);
        
        Item = item;
    }

    public void SetItemAmount(int amount)
    {
        item.Amount = amount;

        amountText.text = Item.GetAmountText();
    }

    public void SetItemInteractableDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        this.itemInteractableDependencies = itemInteractableDependencies;
    }

    private void SetItem(ItemStack item)
    {
        this.item = item;

        GetComponent<SpriteRenderer>().sprite = Item.ItemDefinition.Sprite;

        if (Item.ItemDefinition.HasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = Item.ItemDefinition.CustomColliderSize;
        }

        if (Item.ItemDefinition.TriggerCollisionPickup)
        {
            GetComponent<BoxCollider2D>().isTrigger =
                Item.ItemDefinition.TriggerCollisionPickup;
        }

        if (Item.ItemDefinition.HasProperty("InteractableType"))
        {
            Type itemInteractableType = Type.GetType(
                Item.ItemDefinition.GetStringProperty("InteractableType"));

            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{Item.ItemDefinition.DisplayName} ItemWorld";

        amountText.text = Item.GetAmountText();
    }
}
