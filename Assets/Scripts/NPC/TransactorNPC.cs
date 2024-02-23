using System.Collections.Generic;
using UnityEngine;

public class TransactorNPC : NPCInteractable
{
    [SerializeField] private List<NPCTransactionScriptableObject> transactions;

    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private PlayerController playerController;
    [Inject] private TransactorNPCUIController transactorNPCUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        transactorNPCUIController.DisplayTransactions(transactions);

        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnInventoryUIClosed;
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        StartWaitThenReturnToDefaultDirectionCouroutine();

        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnInventoryUIClosed;
    }
}
