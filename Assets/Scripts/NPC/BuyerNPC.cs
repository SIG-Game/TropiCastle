using UnityEngine;

public class BuyerNPC : NPCInteractable
{
    [SerializeField] private NPCPurchaseScriptableObject purchase;

    [Inject] private BuyerNPCUIController buyerNPCUIController;
    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        buyerNPCUIController.DisplayPurchase(purchase);

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
