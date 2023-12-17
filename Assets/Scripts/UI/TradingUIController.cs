using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> tradingUIGameObjects;
    [SerializeField] private Button tradeButton;
    [SerializeField] private GameObject tradeItemUIPrefab;
    [SerializeField] private Transform inputItemUIParent;
    [SerializeField] private Transform outputItemUIParent;
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

        UpdateTradeItemUI(inputItemUIParent, trade.InputItems);
        UpdateTradeItemUI(outputItemUIParent, trade.OutputItems);

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;

        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(tradingUIGameObjects);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    private void UpdateTradeItemUI(Transform itemUIParent, List<ItemStackStruct> items)
    {
        foreach (Transform itemUI in itemUIParent)
        {
            Destroy(itemUI.gameObject);
        }

        foreach (ItemStackStruct itemStruct in items)
        {
            ItemStack item = itemStruct.ToClassType();

            GameObject itemUI = Instantiate(tradeItemUIPrefab, itemUIParent);

            Image itemImage = itemUI.transform.GetChild(0).GetComponent<Image>();

            itemImage.sprite = item.itemDefinition.Sprite;

            TextMeshProUGUI itemAmountText =
                itemUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            itemAmountText.text = item.GetAmountText();

            itemUI.GetComponent<TradeItemTooltipController>().Item = item;
        }
    }

    public void TradeButton_OnClick()
    {
        playerInventory.ReplaceItems(
            currentTrade.InputItems.Select(x => x.ToClassType()).ToList(),
            currentTrade.OutputItems.Select(x => x.ToClassType()).ToList());

        SetIsCurrentTradePossible();

        tradeButton.interactable = isCurrentTradePossible;
    }

    private void SetIsCurrentTradePossible()
    {
        Dictionary<int, int> itemIndexToUsedAmount = new Dictionary<int, int>();

        isCurrentTradePossible = true;

        foreach (ItemStackStruct inputItemStruct in currentTrade.InputItems)
        {
            ItemStack inputItem = inputItemStruct.ToClassType();

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
