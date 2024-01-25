using System.Collections.Generic;
using UnityEngine;

public class TransactorNPCUIController : NPCInventoryUIController
{
    [SerializeField] private GameObject transactionUIPrefab;
    [SerializeField] private Transform transactionUIParent;

    private List<NPCTransactionScriptableObject> transactions;

    public void DisplayTransactions(
        List<NPCTransactionScriptableObject> transactions)
    {
        this.transactions = transactions;

        DisplayUI();
    }

    protected override void DisplayUI()
    {
        foreach (Transform transactionUI in transactionUIParent)
        {
            Destroy(transactionUI.gameObject);
        }

        foreach (var transaction in transactions)
        {
            GameObject transactionUI =
                Instantiate(transactionUIPrefab, transactionUIParent);

            var transactionUIController =
                transactionUI.GetComponent<NPCTransactionUIController>();

            transactionUIController.SetUp(transaction);
        }

        base.DisplayUI();
    }
}
