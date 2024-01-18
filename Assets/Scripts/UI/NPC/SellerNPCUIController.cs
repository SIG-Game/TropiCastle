using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellerNPCUIController : NPCInventoryUIController
{
    [SerializeField] private GameObject productUIPrefab;
    [SerializeField] private Transform productUIParent;
    [SerializeField] private MoneyController playerMoneyController;

    private List<NPCProductScriptableObject> products;
    private List<Button> buyButtons;

    protected override void Awake()
    {
        base.Awake();

        buyButtons = new List<Button>();
    }

    public void DisplayProducts(List<NPCProductScriptableObject> products)
    {
        this.products = products;

        buyButtons.Clear();

        foreach (Transform productUI in productUIParent)
        {
            Destroy(productUI.gameObject);
        }

        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];

            GameObject productUI = Instantiate(productUIPrefab, productUIParent);

            productUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{product.Item} For {product.Cost} Money";

            var buyButton = productUI.transform.GetChild(1).GetComponent<Button>();

            int iCopy = i;
            buyButton.onClick.AddListener(() => BuyButton_OnClick(iCopy));

            buyButton.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "Buy";

            buyButtons.Add(buyButton);
        }

        DisplayUI();
    }

    public void BuyButton_OnClick(int productIndex)
    {
        var product = products[productIndex];

        if (playerMoneyController.Money >= product.Cost &&
            playerInventory.CanAddItem(product.Item))
        {
            playerInventory.AddItem(product.Item);

            playerMoneyController.Money -= product.Cost;
        }
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        buyButtons.ForEach(x => x.interactable = false);

    protected override void InventoryUIHeldItemController_OnHidden() =>
        buyButtons.ForEach(x => x.interactable = true);
}
