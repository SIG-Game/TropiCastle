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

    protected override string GetTransactionText(int index)
    {
        var purchase = purchases[index];

        return $"{purchase.Item} For {purchase.Payment} Money";
    }

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
