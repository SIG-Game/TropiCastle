using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : MonoBehaviour
{
    [SerializeField] private GameObject tradingUI;
    [SerializeField] private Button tradeButton;
    [SerializeField] private Image inputItemImage;
    [SerializeField] private Image outputItemImage;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PauseController pauseController;

    private NPCTradeScriptableObject currentTrade;
    private int inputItemStackIndex;

    private void Awake()
    {
        inputItemStackIndex = -1;
    }

    // Must run after the PauseMenu Update method so that an Escape key press
    // used to close the trading UI isn't reused to open the pause menu
    private void Update()
    {
        if (tradingUI.activeInHierarchy &&
            CloseTradingUIInputPressed())
        {
            CloseTradingUI();
        }
    }

    public void DisplayTrade(NPCTradeScriptableObject trade)
    {
        currentTrade = trade;

        inputItemImage.sprite = trade.InputItem.sprite;
        outputItemImage.sprite = trade.OutputItem.itemData.sprite;

        UpdateTradeButtonInteractable();

        PauseController.Instance.GamePaused = true;

        tradingUI.SetActive(true);
    }

    public void TradeButton_OnClick()
    {
        playerInventory.DecrementItemStackAtIndex(inputItemStackIndex);
        playerInventory.AddItem(currentTrade.OutputItem);

        UpdateTradeButtonInteractable();
    }

    private void UpdateTradeButtonInteractable()
    {
        tradeButton.interactable = IsCurrentTradePossible();
    }

    private bool CloseTradingUIInputPressed()
    {
        bool closeUsingEscapeKey = Input.GetKeyDown(KeyCode.Escape);

        bool closeUsingInteractAction =
            inputManager.GetInteractButtonDownIfUnusedThisFrame();

        return closeUsingEscapeKey || closeUsingInteractAction;
    }

    private void CloseTradingUI()
    {
        tradingUI.SetActive(false);

        PauseController.Instance.GamePaused = false;

        currentTrade = null;
        inputItemStackIndex = -1;
    }

    private bool IsCurrentTradePossible()
    {
        List<ItemWithAmount> itemList = playerInventory.GetItemList();

        inputItemStackIndex = itemList.FindIndex(
            x => x.itemData == currentTrade.InputItem);

        if (inputItemStackIndex == -1)
        {
            return false;
        }

        return playerInventory.CanAddItem(currentTrade.OutputItem);
    }
}
