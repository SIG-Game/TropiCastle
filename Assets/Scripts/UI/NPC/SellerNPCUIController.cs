using System;
using System.Collections.Generic;

public class SellerNPCUIController : NPCTransactionUIController
{
    private List<NPCProductScriptableObject> products;

    protected override int TransactionCount => products.Count;

    public void DisplayProducts(List<NPCProductScriptableObject> products)
    {
        this.products = products;

        DisplayUI();
    }

    protected override string GetTransactionItemText(int index) =>
        products[index].Item.ToString();

    protected override string GetTransactionMoneyText(int index) =>
        products[index].Cost.ToString();

    protected override Action GetButtonOnClickListener(int index) =>
        () =>
        {
            var product = products[index];

            if (playerMoneyController.Money >= product.Cost &&
                playerInventory.CanAddItem(product.Item))
            {
                playerInventory.AddItem(product.Item);

                playerMoneyController.Money -= product.Cost;
            }
        };
}
