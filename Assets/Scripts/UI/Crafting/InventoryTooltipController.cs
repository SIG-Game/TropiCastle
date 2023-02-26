using TMPro;
using UnityEngine;

public class InventoryTooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;

    private RectTransform inventoryTooltipRectTransform;
    private bool tooltipIsVisible;

    public static InventoryTooltipController Instance;

    private void Awake()
    {
        Instance = this;

        inventoryTooltipRectTransform = GetComponent<RectTransform>();
        tooltipIsVisible = false;

        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void Update()
    {
        if (tooltipIsVisible)
        {
            UpdateInventoryTooltipPosition();
        }
    }

    private void OnDestroy()
    {
        Instance = null;

        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    public void ShowTooltipWithText(string text)
    {
        tooltipText.text = text;
        tooltipIsVisible = true;
        UpdateInventoryTooltipPosition();
    }

    public void HideTooltip()
    {
        tooltipText.text = string.Empty;
        tooltipIsVisible = false;
    }

    private void UpdateInventoryTooltipPosition()
    {
        inventoryTooltipRectTransform.anchoredPosition =
            MouseCanvasPositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        HideTooltip();
    }
}
