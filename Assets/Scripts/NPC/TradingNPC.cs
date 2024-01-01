using UnityEngine;

public class TradingNPC : NPCInteractable
{
    [SerializeField] private NPCTradeScriptableObject trade;

    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private TradingUIController tradingUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        tradingUIController.DisplayTrade(trade);

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
