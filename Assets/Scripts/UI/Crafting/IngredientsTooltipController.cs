using TMPro;
using UnityEngine;

public class IngredientsTooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;

    private RectTransform ingredientsTooltipRectTransform;
    private bool tooltipIsVisible;

    private void Awake()
    {
        ingredientsTooltipRectTransform = GetComponent<RectTransform>();
        tooltipIsVisible = false;

        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void Update()
    {
        if (tooltipIsVisible)
        {
            UpdateIngredientsTooltipPosition();
        }
    }

    private void OnDestroy()
    {
        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    public void ShowTooltipWithText(string text)
    {
        tooltipText.text = text;
        tooltipIsVisible = true;
        UpdateIngredientsTooltipPosition();
    }

    public void HideTooltip()
    {
        tooltipText.text = string.Empty;
        tooltipIsVisible = false;
    }

    private void UpdateIngredientsTooltipPosition()
    {
        ingredientsTooltipRectTransform.anchoredPosition =
            MouseCanvasPositionHelper.GetMouseCanvasPosition(canvasRectTransform);
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        HideTooltip();
    }
}
