using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyerNPCUIController : NPCInventoryUIController
{
    [SerializeField] private GameObject purchaseUIPrefab;
    [SerializeField] private Transform purchaseUIParent;
    [SerializeField] private MoneyController playerMoneyController;

    private List<NPCPurchaseScriptableObject> purchases;
    private List<Button> sellButtons;

    protected override void Awake()
    {
        base.Awake();

        sellButtons = new List<Button>();
    }

    public void DisplayPurchases(List<NPCPurchaseScriptableObject> purchases)
    {
        this.purchases = purchases;

        sellButtons.Clear();

        foreach (Transform purchaseUI in purchaseUIParent)
        {
            Destroy(purchaseUI.gameObject);
        }

        for (int i = 0; i < purchases.Count; ++i)
        {
            var purchase = purchases[i];

            GameObject purchaseUI = Instantiate(purchaseUIPrefab, purchaseUIParent);

            purchaseUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{purchase.Item} For {purchase.Payment} Money";

            var sellButton = purchaseUI.transform.GetChild(1).GetComponent<Button>();

            int iCopy = i;
            sellButton.onClick.AddListener(() => SellButton_OnClick(iCopy));

            sellButton.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "Sell";

            sellButtons.Add(sellButton);
        }

        DisplayUI();
    }

    public void SellButton_OnClick(int purchaseIndex)
    {
        var purchase = purchases[purchaseIndex];

        if (playerInventory.CanRemoveItem(purchase.Item))
        {
            playerInventory.RemoveItem(purchase.Item);

            playerMoneyController.Money += purchase.Payment;
        }
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        sellButtons.ForEach(x => x.interactable = false);

    protected override void InventoryUIHeldItemController_OnHidden() =>
        sellButtons.ForEach(x => x.interactable = true);
}
