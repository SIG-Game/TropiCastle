using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : NPCInventoryUIController
{
    [SerializeField] private Button tradeButton;
    [SerializeField] private GameObject tradeItemUIPrefab;
    [SerializeField] private Transform inputItemUIParent;
    [SerializeField] private Transform outputItemUIParent;

    private NPCTradeScriptableObject currentTrade;
    private bool isCurrentTradePossible;

    public void DisplayTrade(NPCTradeScriptableObject trade)
    {
        currentTrade = trade;

        UpdateTradeItemUI(inputItemUIParent, trade.InputItems);
        UpdateTradeItemUI(outputItemUIParent, trade.OutputItems);

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;

        DisplayUI();
    }

    private void UpdateTradeItemUI(Transform itemUIParent, List<ItemStack> items)
    {
        foreach (Transform itemUI in itemUIParent)
        {
            Destroy(itemUI.gameObject);
        }

        foreach (ItemStack item in items)
        {
            GameObject itemUI = Instantiate(tradeItemUIPrefab, itemUIParent);

            Image itemImage = itemUI.transform.GetChild(0).GetComponent<Image>();

            itemImage.sprite = item.ItemDefinition.Sprite;

            TextMeshProUGUI itemAmountText =
                itemUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            itemAmountText.text = item.GetAmountText();

            itemUI.GetComponent<TradeItemTooltipController>().Item = item;
        }
    }

    public void TradeButton_OnClick()
    {
        playerInventory.ReplaceItems(
            currentTrade.InputItems, currentTrade.OutputItems);

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;
    }

    private void SetIsCurrentTradePossible()
    {
        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        isCurrentTradePossible = true;

        foreach (ItemStack inputItem in currentTrade.InputItems)
        {
            bool playerHasInputItem = playerInventory
                .HasReplacementInputItem(itemIndexToUsedAmount, inputItem);

            isCurrentTradePossible = isCurrentTradePossible && playerHasInputItem;
        }
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        tradeButton.interactable = false;

    protected override void InventoryUIHeldItemController_OnHidden() =>
        tradeButton.interactable = isCurrentTradePossible;
}