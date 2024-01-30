using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : NPCInventoryUIController
{
    [SerializeField] private Button tradeButton;
    [SerializeField] private GameObject tradeItemUIPrefab;
    [SerializeField] private Transform inputItemUIParent;
    [SerializeField] private Transform outputItemUIParent;

    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;

    private NPCTradeScriptableObject currentTrade;

    protected override void Awake()
    {
        base.Awake();

        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden +=
            UpdateTradeButtonInteractability;
        playerInventory.OnItemChangedAtIndex +=
            PlayerInventory_OnItemChangedAtIndex;
    }

    private void OnDestroy()
    {
        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -=
            UpdateTradeButtonInteractability;
        playerInventory.OnItemChangedAtIndex -=
            PlayerInventory_OnItemChangedAtIndex;
    }

    public void DisplayTrade(NPCTradeScriptableObject trade)
    {
        currentTrade = trade;

        UpdateTradeItemUI(inputItemUIParent, trade.InputItems);
        UpdateTradeItemUI(outputItemUIParent, trade.OutputItems);

        UpdateTradeButtonInteractability();

        DisplayUI();

        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnTradingUIClosed;
    }

    private void UpdateTradeItemUI(
        Transform tradeItemUIParent, List<ItemStack> items)
    {
        foreach (Transform tradeItemUI in tradeItemUIParent)
        {
            Destroy(tradeItemUI.gameObject);
        }

        foreach (ItemStack item in items)
        {
            GameObject tradeItemUI = Instantiate(
                tradeItemUIPrefab, tradeItemUIParent);

            tradeItemUI.GetComponent<NPCTradeItemUIController>().SetUp(item);
        }
    }

    private void UpdateTradeButtonInteractability()
    {
        if (inventoryUIHeldItemController.HoldingItem())
        {
            tradeButton.interactable = false;
        }
        else if (currentTrade != null)
        {
            // Prevent this method from calling itself
            // through PlayerInventory_OnItemChangedAtIndex
            playerInventory.OnItemChangedAtIndex -=
                PlayerInventory_OnItemChangedAtIndex;

            tradeButton.interactable = playerInventory.CanReplaceItems(
                currentTrade.InputItems, currentTrade.OutputItems);

            playerInventory.OnItemChangedAtIndex +=
                PlayerInventory_OnItemChangedAtIndex;
        }
    }

    public void TradeButton_OnClick() =>
        playerInventory.ReplaceItems(
            currentTrade.InputItems, currentTrade.OutputItems);

    private void InventoryUIManager_OnTradingUIClosed()
    {
        currentTrade = null;

        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnTradingUIClosed;
    }

    private void InventoryUIHeldItemController_OnItemHeld() =>
        tradeButton.interactable = false;

    private void PlayerInventory_OnItemChangedAtIndex(ItemStack _, int _1) =>
        UpdateTradeButtonInteractability();
}
