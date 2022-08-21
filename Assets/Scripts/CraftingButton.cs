using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingButton : MonoBehaviour, IPointerClickHandler
{
    public Crafting crafting;
    public List<Item> ingredients;
    public Item resultItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            crafting.CraftItem(ingredients, resultItem);
        }
    }
}
