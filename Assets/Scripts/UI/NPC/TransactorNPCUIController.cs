using System.Collections.Generic;
using UnityEngine;

public class TransactorNPCUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> transactorNPCUIGameObjects;
    [SerializeField] private GameObject transactionUIPrefab;
    [SerializeField] private Transform transactionUIParent;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    [Inject] private InventoryUIManager inventoryUIManager;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void DisplayTransactions(
        List<NPCTransactionScriptableObject> transactions)
    {
        foreach (Transform transactionUI in transactionUIParent)
        {
            Destroy(transactionUI.gameObject);
        }

        foreach (var transaction in transactions)
        {
            GameObject transactionUI =
                Instantiate(transactionUIPrefab, transactionUIParent);

            transactionUI.GetComponent<NPCTransactionUIController>()
                .SetUp(transaction);
        }

        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.ShowInventoryUI(transactorNPCUIGameObjects);
    }
}
