using UnityEngine;

public class TradingNPC : NPCInteractable
{
    [SerializeField] private NPCTradeScriptableObject trade;

    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private PlayerController playerController;
    [Inject] private TradingUIController tradingUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        tradingUIController.DisplayTrade(trade);

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
