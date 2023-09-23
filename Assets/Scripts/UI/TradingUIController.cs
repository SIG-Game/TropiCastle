using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> tradingUIGameObjects;
    [SerializeField] private Button tradeButton;
    [SerializeField] private GameObject inputItemUIPrefab;
    [SerializeField] private Transform inputItemUIParent;
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

        foreach (Transform inputItemUI in inputItemUIParent)
        {
            Destroy(inputItemUI.gameObject);
        }

        foreach (ItemWithAmount inputItem in trade.InputItems)
        {
            GameObject inputItemUI =
                Instantiate(inputItemUIPrefab, inputItemUIParent);

            inputItemUI.GetComponent<Image>().sprite =
                inputItem.itemDefinition.sprite;

            TextMeshProUGUI inputItemAmountText =
                inputItemUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            inputItemAmountText.text = inputItem.GetAmountText();
        }

        outputItemImage.sprite = trade.OutputItem.itemDefinition.sprite;
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
            currentTrade.InputItems,
            currentTrade.OutputItem);

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;
    }

    private void SetIsCurrentTradePossible()
    {
        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        isCurrentTradePossible = true;

        foreach (ItemWithAmount inputItem in currentTrade.InputItems)
        {
            bool playerHasInputItem = playerInventory.HasReplacementInputItem(
               itemIndexToUsedAmount, inputItem);

            isCurrentTradePossible = isCurrentTradePossible && playerHasInputItem;
        }
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
