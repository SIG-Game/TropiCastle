using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NPCTransactionType;

public class NPCTransactionUIController : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private RectTransform arrow;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject("PlayerInventory")] private Inventory playerInventory;
    [Inject("PlayerMoneyController")] private MoneyController playerMoneyController;

    public Button Button => button;

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

    public void SetUp(NPCTransactionScriptableObject transaction)
    {
        if (transaction.TransactionType == Product)
        {
            buttonText.text = "Buy";

            button.onClick.AddListener(new UnityAction(
                GetBuyButtonOnClickListener(transaction)));
        }
        else
        {
            buttonText.text = "Sell";

            button.onClick.AddListener(new UnityAction(
                GetSellButtonOnClickListener(transaction)));

            arrow.localScale = new Vector3(-1f, 1f, 1f);
        }

        moneyText.text = transaction.Money.ToString();
        itemAmountText.text = transaction.Item.Amount.ToString();

        itemImage.sprite = transaction.Item.ItemDefinition.Sprite;

        itemImage.GetComponent<ItemTooltipController>().Item = transaction.Item;
    }

    private Action GetBuyButtonOnClickListener(
        NPCTransactionScriptableObject transaction) =>
        () =>
        {
            if (playerMoneyController.Money >= transaction.Money &&
                playerInventory.CanAddItem(transaction.Item))
            {
                playerInventory.AddItem(transaction.Item);

                playerMoneyController.Money -= transaction.Money;
            }
        };

    private Action GetSellButtonOnClickListener(
        NPCTransactionScriptableObject transaction) =>
        () =>
        {
            if (playerInventory.CanRemoveItem(transaction.Item))
            {
                playerInventory.RemoveItem(transaction.Item);

                playerMoneyController.Money += transaction.Money;
            }
        };

    private void InventoryUIHeldItemController_OnItemHeld() =>
        button.interactable = false;

    private void InventoryUIHeldItemController_OnHidden() =>
        button.interactable = true;
}
