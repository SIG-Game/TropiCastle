using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellerNPCUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> sellerNPCUIGameObjects;
    [SerializeField] private TextMeshProUGUI productText;
    [SerializeField] private Button buyButton;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private MoneyController playerMoneyController;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject] private InventoryUIManager inventoryUIManager;

    private NPCProductScriptableObject product;

    private void Awake()
    {
        this.InjectDependencies();

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

    public void DisplayProduct(NPCProductScriptableObject product)
    {
        this.product = product;

        productText.text = $"{product.Item} For {product.Cost} Money";

        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

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

    private void InventoryUIHeldItemController_OnItemHeld() =>
        buyButton.interactable = false;

    private void InventoryUIHeldItemController_OnHidden() =>
        buyButton.interactable = true;
}
