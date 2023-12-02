using System;
using TMPro;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private ItemStack item;
    [SerializeField] private TMP_Text amountText;

    public ItemStack Item
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
        Item.amount = amount;

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

        GetComponent<SpriteRenderer>().sprite = Item.itemDefinition.Sprite;

        if (Item.itemDefinition.HasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = Item.itemDefinition.CustomColliderSize;
        }

        if (Item.itemDefinition.TriggerCollisionPickup)
        {
            GetComponent<BoxCollider2D>().isTrigger =
                Item.itemDefinition.TriggerCollisionPickup;
        }

        if (Item.itemDefinition.HasProperty("InteractableType"))
        {
            Type itemInteractableType = Type.GetType(
                Item.itemDefinition.GetStringProperty("InteractableType"));

            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{Item.itemDefinition.DisplayName} ItemWorld";

        amountText.text = Item.GetAmountText();
    }
}
