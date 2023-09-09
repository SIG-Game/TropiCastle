using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> tradingUIGameObjects;
    [SerializeField] private Button tradeButton;
    [SerializeField] private Image inputItemImage;
    [SerializeField] private TextMeshProUGUI inputItemAmountText;
    [SerializeField] private Image outputItemImage;
    [SerializeField] private TextMeshProUGUI outputItemAmountText;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    private NPCTradeScriptableObject currentTrade;
    private bool isCurrentTradePossible;

    private void Awake()
    {
        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden +=
            InventoryUIHeldItemController_OnHidden;
    }

    private void OnDestroy()
    {
        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -=
            InventoryUIHeldItemController_OnHidden;
    }

    public void DisplayTrade(NPCTradeScriptableObject trade)
    {
        currentTrade = trade;

        inputItemImage.sprite = trade.InputItem.itemDefinition.sprite;
        outputItemImage.sprite = trade.OutputItem.itemDefinition.sprite;

        inputItemAmountText.text = trade.InputItem.GetAmountText();
        outputItemAmountText.text = trade.OutputItem.GetAmountText();

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;

        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(tradingUIGameObjects);
        inventoryUIManager.SetCanCloseUsingInteractAction(true);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    public void TradeButton_OnClick()
    {
        playerInventory.ReplaceItems(
            new List<ItemWithAmount> { currentTrade.InputItem },
            currentTrade.OutputItem);

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;
    }

    private void SetIsCurrentTradePossible()
    {
        isCurrentTradePossible = playerInventory.HasReplacementInputItem(
            new Dictionary<int, int>(), currentTrade.InputItem);
    }

    private void InventoryUIHeldItemController_OnItemHeld()
    {
        tradeButton.interactable = false;
    }

    private void InventoryUIHeldItemController_OnHidden()
    {
        tradeButton.interactable = isCurrentTradePossible;
    }
}
