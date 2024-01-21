using System.Collections.Generic;
using UnityEngine;

public class BuyerNPC : NPCInteractable
{
    [SerializeField] private List<NPCTransactionScriptableObject> purchases;

    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private NPCTransactionUIController npcTransactionUIController;
    [Inject] private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        npcTransactionUIController.DisplayTransactions(purchases);

        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnInventoryUIClosed;
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        directionController.UseDefaultDirection();

        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnInventoryUIClosed;
    }
}
