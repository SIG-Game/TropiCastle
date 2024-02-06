using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour, IElementWithTooltip
{
    [SerializeField] private Button craftingButton;
    [SerializeField] private CraftingRecipeScriptableObject craftingRecipe;
    [SerializeField] private Image craftingButtonImage;

    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject("PlayerInventory")] private Inventory playerInventory;

    private string resultItemTooltipText;

    private void Awake()
    {
        this.InjectDependencies();

        // This could be changed to not be set at runtime
        // It this wasn't set at runtime, an old item tooltip format might get cached
        resultItemTooltipText = $"Result:\n" +
            craftingRecipe.ResultItem.ItemDefinition.GetTooltipText();

        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden +=
            InventoryUIHeldItemController_OnHidden;
    }

    private void OnDestroy()
    {
        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -=
            InventoryUIHeldItemController_OnHidden;
    }

    public void CraftingButton_OnClick()
    {
        playerInventory.ReplaceItems(
            craftingRecipe.Ingredients, craftingRecipe.ResultItem);
    }

    public void SetUpCraftingButton(CraftingRecipeScriptableObject craftingRecipe)
    {
        this.craftingRecipe = craftingRecipe;

        craftingButtonImage.sprite = craftingRecipe.ResultItem.ItemDefinition.Sprite;
    }

    public string GetTooltipText()
    {
        StringBuilder ingredientsStringBuilder = new StringBuilder("Ingredients:\n");

        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        foreach (ItemStack ingredient in craftingRecipe.Ingredients)
        {
            bool playerHasIngredient = playerInventory
                .HasReplacementInputItem(itemIndexToUsedAmount, ingredient);

            ingredientsStringBuilder.Append(
                playerHasIngredient ? "<color=#028D00>" : "<color=#D20000>");
            ingredientsStringBuilder.Append($"- {ingredient} ");
            ingredientsStringBuilder.Append(playerHasIngredient ? "Y" : "N");
            ingredientsStringBuilder.Append("</color>");
            ingredientsStringBuilder.AppendLine();
        }

        return ingredientsStringBuilder.ToString();
    }

    public string GetAlternateTooltipText() => resultItemTooltipText;

    private void InventoryUIHeldItemController_OnItemHeld()
    {
        craftingButton.interactable = false;
    }

    private void InventoryUIHeldItemController_OnHidden()
    {
        craftingButton.interactable = true;
    }
}
