using System;
using TMPro;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private ItemStack item;
    [SerializeField] private TMP_Text amountText;

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

        if (item.itemDefinition.HasProperty("InteractableType"))
        {
            Type itemInteractableType = Type.GetType(
                item.itemDefinition.GetStringProperty("InteractableType"));

            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{item.itemDefinition.DisplayName} ItemWorld";

        amountText.text = item.GetAmountText();
    }

    public ItemStack GetItem() => item;
}
