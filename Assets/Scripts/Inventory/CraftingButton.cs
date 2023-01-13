using System.Collections.Generic;
using UnityEngine;

public class CraftingButton : MonoBehaviour
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private List<ItemWithAmount> ingredients;
    [SerializeField] private ItemWithAmount resultItem;

    public void CraftingButton_OnClick()
    {
        crafting.CraftItem(ingredients, resultItem);
    }
}
