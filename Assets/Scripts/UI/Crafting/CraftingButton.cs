using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button craftingButton;
    [SerializeField] private CraftingButtonDependencies craftingButtonDependencies;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private Image craftingButtonImage;

    private Tooltip tooltipTextWithPriority;
    private string resultItemTooltipText;

    private void Awake()
    {
        // This could be changed to not be set at runtime
        // It this wasn't set at runtime, an old item tooltip format might get cached
        resultItemTooltipText = $"Result:\n" +
            craftingRecipe.resultItem.itemData.GetTooltipText();

        InventoryUIHeldItemController.OnStartedHoldingItem +=
            InventoryUIHeldItemController_OnStartedHoldingItem;
        InventoryUIHeldItemController.OnStoppedHoldingItem +=
            InventoryUIHeldItemController_OnStoppedHoldingItem;
    }

    private void OnDestroy()
    {
        InventoryUIHeldItemController.OnStartedHoldingItem -=
            InventoryUIHeldItemController_OnStartedHoldingItem;
        InventoryUIHeldItemController.OnStoppedHoldingItem -=
            InventoryUIHeldItemController_OnStoppedHoldingItem;
    }

    public void CraftingButton_OnClick()
    {
        if (!InventoryUIHeldItemController.Instance.HoldingItem())
        {
            craftingButtonDependencies.GetCrafting().CraftItem(craftingRecipe);
        }
    }

    public void SetUpCraftingButton(CraftingButtonDependencies craftingButtonDependencies,
        CraftingRecipeScriptableObject craftingRecipe)
    {
        this.craftingButtonDependencies = craftingButtonDependencies;
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

        List<ItemWithAmount> playerInventoryItemList =
            craftingButtonDependencies.GetPlayerInventory().GetItemList();

        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        foreach (ItemWithAmount ingredient in craftingRecipe.ingredients)
        {
            bool playerHasIngredient = craftingButtonDependencies.GetCrafting()
                .TryFindIngredient(playerInventoryItemList, itemIndexToUsedAmount, ingredient);

            ingredientsStringBuilder.Append(playerHasIngredient ? "<color=#00FF00>" : "<color=#FF0000>");
            ingredientsStringBuilder.Append($"- {ingredient.amount} {ingredient.itemData.name}: ");
            ingredientsStringBuilder.Append(playerHasIngredient ? "Y" : "N");
            ingredientsStringBuilder.Append("</color>");
            ingredientsStringBuilder.AppendLine();
        }

        return ingredientsStringBuilder.ToString();
    }

    private void InventoryUIHeldItemController_OnStartedHoldingItem()
    {
        craftingButton.interactable = false;
    }

    private void InventoryUIHeldItemController_OnStoppedHoldingItem()
    {
        craftingButton.interactable = true;
    }
}
