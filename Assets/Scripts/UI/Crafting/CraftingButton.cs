﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Crafting crafting;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private Image craftingButtonImage;
    [SerializeField] private Inventory playerInventory;

    private Tooltip tooltipTextWithPriority;
    private string resultItemTooltipText;

    private void Awake()
    {
        // This could be changed to not be set at runtime
        // It this wasn't set at runtime, an old item tooltip format might get cached
        resultItemTooltipText = $"Result:\n" +
            $"{InventoryUITooltipController.GetItemTooltipText(craftingRecipe.resultItem.itemData)}";
    }

    public void CraftingButton_OnClick()
    {
        if (!InventoryUIHeldItemController.Instance.HoldingItem())
        {
            crafting.CraftItem(craftingRecipe);
        }
    }

    public void SetUpCraftingButton(Crafting crafting, Inventory playerInventory,
        CraftingRecipeScriptableObject craftingRecipe)
    {
        this.crafting = crafting;
        this.playerInventory = playerInventory;
        this.craftingRecipe = craftingRecipe;

        craftingButtonImage.sprite = craftingRecipe.resultItem.itemData.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string craftingButtonTooltipText = GetIngredientsAsString();
        tooltipTextWithPriority = new Tooltip(craftingButtonTooltipText, resultItemTooltipText, 0);
        InventoryUITooltipController.Instance.AddTooltipTextWithPriority(tooltipTextWithPriority);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(tooltipTextWithPriority);
    }

    private string GetIngredientsAsString()
    {
        StringBuilder ingredientsStringBuilder = new StringBuilder("Ingredients:\n");

        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        // Get indexes of inventory items used when checking if the player's inventory has an ingredient
        // so that an inventory item is not reused when multiple ingredients have the same item name
        List<int> itemIndexesUsed = new List<int>();

        foreach (ItemWithAmount ingredient in craftingRecipe.ingredients)
        {
            bool playerHasIngredient = false;

            for (int i = 0; i < playerInventoryItemList.Count; ++i)
            {
                ItemWithAmount currentItem = playerInventoryItemList[i];

                if (currentItem.itemData.name == ingredient.itemData.name && !itemIndexesUsed.Contains(i))
                {
                    playerHasIngredient = true;
                    itemIndexesUsed.Add(i);
                    break;
                }
            }

            ingredientsStringBuilder.Append(playerHasIngredient ? "<color=#00FF00>" : "<color=#FF0000>");
            ingredientsStringBuilder.Append($"- {ingredient.itemData.name}: ");
            ingredientsStringBuilder.Append(playerHasIngredient ? "Y" : "N");
            ingredientsStringBuilder.Append("</color>");
            ingredientsStringBuilder.AppendLine();
        }

        return ingredientsStringBuilder.ToString();
    }
}
