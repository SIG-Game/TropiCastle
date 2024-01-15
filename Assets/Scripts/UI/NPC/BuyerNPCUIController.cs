using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyerNPCUIController : NPCInventoryUIController
{
    [SerializeField] private TextMeshProUGUI purchaseText;
    [SerializeField] private Button sellButton;
    [SerializeField] private MoneyController playerMoneyController;

    private NPCPurchaseScriptableObject purchase;

    public void DisplayPurchase(NPCPurchaseScriptableObject purchase)
    {
        this.purchase = purchase;

        purchaseText.text = $"{purchase.Item} For {purchase.Payment} Money";

        DisplayUI();
    }

    public void SellButton_OnClick()
    {
        if (playerInventory.CanRemoveItem(purchase.Item))
        {
            playerInventory.RemoveItem(purchase.Item);

            playerMoneyController.Money += purchase.Payment;
        }
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        sellButton.interactable = false;

    protected override void InventoryUIHeldItemController_OnHidden() =>
        sellButton.interactable = true;
}
