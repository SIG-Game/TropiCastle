using System;
using System.Collections.Generic;

public class BuyerNPCUIController : NPCTransactionUIController
{
    private List<NPCPurchaseScriptableObject> purchases;

    protected override int TransactionCount => purchases.Count;

    public void DisplayPurchases(List<NPCPurchaseScriptableObject> purchases)
    {
        this.purchases = purchases;

        DisplayUI();
    }

    protected override string GetTransactionItemText(int index) =>
        purchases[index].Item.ToString();

    protected override string GetTransactionMoneyText(int index) =>
        purchases[index].Payment.ToString();

    protected override Action GetButtonOnClickListener(int index) =>
        () =>
        {
            var purchase = purchases[index];

            if (playerInventory.CanRemoveItem(purchase.Item))
            {
                playerInventory.RemoveItem(purchase.Item);

                playerMoneyController.Money += purchase.Payment;
            }
        };
}
