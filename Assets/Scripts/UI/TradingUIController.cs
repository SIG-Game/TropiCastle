using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : MonoBehaviour
{
    [SerializeField] private GameObject tradeUI;
    [SerializeField] private Button tradeButton;
    [SerializeField] private Image inputItemImage;
    [SerializeField] private Image outputItemImage;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InventoryUIHeldItemController inventoryUIHeldItemController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    private List<GameObject> tradingUIGameObjects;
    private NPCTradeScriptableObject currentTrade;
    private int inputItemStackIndex;
    private bool isCurrentTradePossible;

    private void Awake()
    {
        tradingUIGameObjects = new List<GameObject>
        {
            tradeUI,
            playerInventoryUI.gameObject
        };

        inputItemStackIndex = -1;

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

        inputItemImage.sprite = trade.InputItem.sprite;
        outputItemImage.sprite = trade.OutputItem.itemData.sprite;

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;

        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(tradingUIGameObjects);
        inventoryUIManager.SetCanCloseUsingInteractAction(true);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    public void TradeButton_OnClick()
    {
        playerInventory.DecrementItemStackAtIndex(inputItemStackIndex);
        playerInventory.AddItem(currentTrade.OutputItem);

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;
    }

    private void SetIsCurrentTradePossible()
    {
        List<ItemWithAmount> itemList = playerInventory.GetItemList();

        inputItemStackIndex = itemList.FindIndex(
            x => x.itemData == currentTrade.InputItem);

        isCurrentTradePossible =
            inputItemStackIndex != -1 &&
            playerInventory.CanAddItem(currentTrade.OutputItem);
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
