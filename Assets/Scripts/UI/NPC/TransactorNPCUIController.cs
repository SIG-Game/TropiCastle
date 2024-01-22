using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransactorNPCUIController : NPCInventoryUIController
{
    [SerializeField] private GameObject transactionUIPrefab;
    [SerializeField] private Transform transactionUIParent;
    [SerializeField] protected MoneyController playerMoneyController;

    private List<Button> buttons;
    private List<NPCTransactionScriptableObject> transactions;

    protected override void Awake()
    {
        base.Awake();

        buttons = new List<Button>();
    }

    public void DisplayTransactions(
        List<NPCTransactionScriptableObject> transactions)
    {
        this.transactions = transactions;

        DisplayUI();
    }

    protected override void DisplayUI()
    {
        buttons.Clear();

        foreach (Transform transactionUI in transactionUIParent)
        {
            Destroy(transactionUI.gameObject);
        }

        foreach (var transaction in transactions)
        {
            GameObject transactionUI =
                Instantiate(transactionUIPrefab, transactionUIParent);

            var button = transactionUI.transform.GetChild(0).GetComponent<Button>();

            button.onClick.AddListener(
                new UnityAction(GetButtonOnClickListener(transaction)));

            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                transaction.TransactionType == NPCTransactionType.Product ? "Buy" : "Sell";

            buttons.Add(button);

            transactionUI.transform.GetChild(1).GetChild(1)
                .GetComponent<TextMeshProUGUI>().text = transaction.Money.ToString();

            if (transaction.TransactionType == NPCTransactionType.Purchase)
            {
                transactionUI.transform.GetChild(2)
                    .GetComponent<RectTransform>().localScale = new Vector3(-1f, 1f, 1f);
            }

            transactionUI.transform.GetChild(3)
                .GetComponent<TextMeshProUGUI>().text = transaction.Item.ToString();
        }

        base.DisplayUI();
    }

    private Action GetButtonOnClickListener(
        NPCTransactionScriptableObject transaction) =>
        () =>
        {
            if (transaction.TransactionType == NPCTransactionType.Product)
            {
                if (playerMoneyController.Money >= transaction.Money &&
                    playerInventory.CanAddItem(transaction.Item))
                {
                    playerInventory.AddItem(transaction.Item);

                    playerMoneyController.Money -= transaction.Money;
                }
            }
            else
            {
                if (playerInventory.CanRemoveItem(transaction.Item))
                {
                    playerInventory.RemoveItem(transaction.Item);

                    playerMoneyController.Money += transaction.Money;
                }
            }
        };

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        buttons.ForEach(x => x.interactable = false);

    protected override void InventoryUIHeldItemController_OnHidden() =>
        buttons.ForEach(x => x.interactable = true);
}
