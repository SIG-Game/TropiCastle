using UnityEngine;

public class TradingNPC : NPCInteractable
{
    [SerializeField] private NPCTradeScriptableObject trade;
    [SerializeField] private TradingUIController tradingUIController;

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        tradingUIController.DisplayTrade(trade);
    }
}
