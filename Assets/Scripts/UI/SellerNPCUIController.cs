using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellerNPCUIController : NPCInventoryUIController
{
    [SerializeField] private TextMeshProUGUI productText;
    [SerializeField] private Button buyButton;
    [SerializeField] private MoneyController playerMoneyController;

    private NPCProductScriptableObject product;

    public void DisplayProduct(NPCProductScriptableObject product)
    {
        this.product = product;

        productText.text = $"{product.Item} For {product.Cost} Money";

        DisplayUI();
    }

    public void BuyButton_OnClick()
    {
        if (playerMoneyController.Money >= product.Cost &&
            playerInventory.CanAddItem(product.Item))
        {
            playerInventory.AddItem(product.Item);

            playerMoneyController.Money -= product.Cost;
        }
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        buyButton.interactable = false;

    protected override void InventoryUIHeldItemController_OnHidden() =>
        buyButton.interactable = true;
}
