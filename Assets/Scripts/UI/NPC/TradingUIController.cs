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
    private bool skipInventoryChangeEventHandler;

    public void DisplayTrade(NPCTradeScriptableObject trade)
    {
        currentTrade = trade;

        UpdateTradeItemUI(inputItemUIParent, trade.InputItems);
        UpdateTradeItemUI(outputItemUIParent, trade.OutputItems);

        UpdateTradeButtonInteractability();

        DisplayUI();

        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden +=
            UpdateTradeButtonInteractability;
        playerInventory.OnItemChangedAtIndex +=
            PlayerInventory_OnItemChangedAtIndex;
        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnTradingUIClosed;
    }

    private void UpdateTradeItemUI(
        Transform tradeItemUIParent, List<ItemStack> items)
    {
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
            skipInventoryChangeEventHandler = true;

            tradeButton.interactable = playerInventory.CanReplaceItems(
                currentTrade.InputItems, currentTrade.OutputItems);

            skipInventoryChangeEventHandler = false;
        }
    }

    public void TradeButton_OnClick() =>
        playerInventory.ReplaceItems(
            currentTrade.InputItems, currentTrade.OutputItems);

    private void InventoryUIManager_OnTradingUIClosed()
    {
        currentTrade = null;

        foreach (Transform inputItemUI in inputItemUIParent)
        {
            Destroy(inputItemUI.gameObject);
        }

        foreach (Transform outputItemUI in outputItemUIParent)
        {
            Destroy(outputItemUI.gameObject);
        }

        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -=
            UpdateTradeButtonInteractability;
        playerInventory.OnItemChangedAtIndex -=
            PlayerInventory_OnItemChangedAtIndex;
        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnTradingUIClosed;
    }

    private void InventoryUIHeldItemController_OnItemHeld() =>
        tradeButton.interactable = false;

    private void PlayerInventory_OnItemChangedAtIndex(ItemStack _, int _1)
    {
        if (!skipInventoryChangeEventHandler)
        {
            UpdateTradeButtonInteractability();
        }
    }
}
