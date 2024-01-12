using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellerNPCUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> sellerNPCUIGameObjects;
    [SerializeField] private TextMeshProUGUI productText;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private MoneyController playerMoneyController;

    [Inject] private InventoryUIManager inventoryUIManager;

    private NPCProductScriptableObject product;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void DisplayProduct(NPCProductScriptableObject product)
    {
        this.product = product;

        productText.text = $"{product.Item} For {product.Cost} Money";

        inventoryUIManager.SetCurrentInventoryUIGameObjects(sellerNPCUIGameObjects);
        inventoryUIManager.EnableCurrentInventoryUI();
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
}
