using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NPCTransactionType;

public class NPCTransactionUIController : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private RectTransform coin;
    [SerializeField] private RectTransform arrow;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject("PlayerInventory")] private Inventory playerInventory;
    [Inject("PlayerMoneyController")] private MoneyController playerMoneyController;

    public Button Button => button;

    private NPCTransactionScriptableObject transaction;

    private void Awake()
    {
        this.InjectDependencies();

        UpdateButtonInteractability();

        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden += UpdateButtonInteractability;
        playerInventory.OnItemChangedAtIndex += 
            PlayerInventory_OnItemChangedAtIndex;

        if (transaction.TransactionType == Product)
        {
            playerMoneyController.OnMoneySet += UpdateButtonInteractability;
        }
    }

    private void OnDestroy()
    {
        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -= UpdateButtonInteractability;
        playerInventory.OnItemChangedAtIndex -=
            PlayerInventory_OnItemChangedAtIndex;
        playerMoneyController.OnMoneySet -= UpdateButtonInteractability;
    }

    public void SetUp(NPCTransactionScriptableObject transaction)
    {
        this.transaction = transaction;

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

            itemImage.transform.SetAsFirstSibling();
            itemAmountText.transform.SetAsFirstSibling();
            moneyText.transform.SetAsLastSibling();
            coin.SetAsLastSibling();
        }

        moneyText.text = transaction.Money.ToString();
        itemAmountText.text = transaction.Item.Amount.ToString();

        itemImage.sprite = transaction.Item.ItemDefinition.Sprite;

        itemImage.GetComponent<ItemTooltipController>().Item = transaction.Item;
    }

    private void UpdateButtonInteractability()
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            button.interactable = false;
        }
        else if (transaction.TransactionType == Product)
        {
            button.interactable =
                playerMoneyController.Money >= transaction.Money &&
                playerInventory.CanAddItem(transaction.Item);
        }
        else
        {
            button.interactable =
                playerInventory.CanRemoveItem(transaction.Item);
        }
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

    private void PlayerInventory_OnItemChangedAtIndex(ItemStack _, int _1) =>
        UpdateButtonInteractability();
}
