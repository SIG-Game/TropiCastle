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

        purchaseText.text = $"1 {purchase.ItemDefinition.DisplayName}" +
            $" For {purchase.Payment} Money";

        DisplayUI();
    }

    public void SellButton_OnClick()
    {
        List<ItemStack> playerItemList = playerInventory.GetItemList();

        int purchaseItemIndex = playerItemList.FindIndex(
            x => x.ItemDefinition == purchase.ItemDefinition);

        if (purchaseItemIndex != -1)
        {
            playerInventory.DecrementItemStackAtIndex(purchaseItemIndex);

            playerMoneyController.Money += purchase.Payment;
        }
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        sellButton.interactable = false;

    protected override void InventoryUIHeldItemController_OnHidden() =>
        sellButton.interactable = true;
}
