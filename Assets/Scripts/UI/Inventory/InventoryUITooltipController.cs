using TMPro;
using UnityEngine;

public class InventoryUITooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;

    private string heldItemTooltipText;
    private string hoveredTooltipText;
    private RectTransform inventoryTooltipRectTransform;
    private bool tooltipIsVisible;

    public static InventoryUITooltipController Instance;

    private void Awake()
    {
        Instance = this;

        heldItemTooltipText = string.Empty;
        hoveredTooltipText = string.Empty;
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

    public void SetHeldItemTooltipText(string heldItemTooltipText)
    {
        this.heldItemTooltipText = heldItemTooltipText;

        ShowTooltipText(this.heldItemTooltipText);
        UpdateInventoryTooltipPosition();
    }

    public void ClearHeldItemTooltipText()
    {
        heldItemTooltipText = string.Empty;

        if (hoveredTooltipText != string.Empty)
        {
            ShowTooltipText(hoveredTooltipText);
            UpdateInventoryTooltipPosition();
        }
        else
        {
            HideTooltip();
        }
    }

    public void SetHoveredTooltipText(string hoveredTooltipText)
    {
        this.hoveredTooltipText = hoveredTooltipText;

        if (heldItemTooltipText == string.Empty)
        {
            ShowTooltipText(this.hoveredTooltipText);
            UpdateInventoryTooltipPosition();
        }
    }

    public void ClearHoveredTooltipText()
    {
        hoveredTooltipText = string.Empty;

        if (heldItemTooltipText == string.Empty)
        {
            HideTooltip();
        }
    }

    public static string GetItemTooltipText(ItemScriptableObject item) =>
        item switch
        {
            HealingItemScriptableObject healingItem =>
                $"{item.name}\nHeals {healingItem.healAmount} Health",
            WeaponItemScriptableObject weaponItem =>
                $"{item.name}\nDeals {weaponItem.damage} Damage",
            { name: "Empty" } => string.Empty,
            _ => item.name
        };

    private void ShowTooltipText(string text)
    {
        tooltipText.text = text;
        tooltipIsVisible = true;
    }

    private void HideTooltip()
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
        ClearHeldItemTooltipText();
        ClearHoveredTooltipText();
    }
}
