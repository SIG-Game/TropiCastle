using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUITooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;

    private List<KeyValuePair<string, int>> tooltipTextsWithPriority;

    private RectTransform inventoryTooltipRectTransform;

    public static InventoryUITooltipController Instance;

    private void Awake()
    {
        Instance = this;

        tooltipTextsWithPriority = new List<KeyValuePair<string, int>>();

        inventoryTooltipRectTransform = GetComponent<RectTransform>();

        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(tooltipText.text))
        {
            UpdateInventoryTooltipPosition();
        }
    }

    private void OnDestroy()
    {
        Instance = null;

        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    public void AddTooltipTextWithPriority(KeyValuePair<string, int> textWithPriority)
    {
        tooltipTextsWithPriority.Add(textWithPriority);
        tooltipText.text = GetTooltipTextWithHighestPriority();
        UpdateInventoryTooltipPosition();
    }

    public void RemoveTooltipTextWithPriority(KeyValuePair<string, int> textWithPriority)
    {
        tooltipTextsWithPriority.Remove(textWithPriority);
        tooltipText.text = GetTooltipTextWithHighestPriority();
    }

    private string GetTooltipTextWithHighestPriority()
    {
        string tooltipTextWithHighestPriority = null;
        int? highestPriority = null;

        foreach (var tooltipTextWithPriority in tooltipTextsWithPriority)
        {
            if (highestPriority == null || tooltipTextWithPriority.Value > highestPriority)
            {
                highestPriority = tooltipTextWithPriority.Value;
                tooltipTextWithHighestPriority = tooltipTextWithPriority.Key;
            }
        }

        return tooltipTextWithHighestPriority ?? string.Empty;
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

    private void UpdateInventoryTooltipPosition()
    {
        inventoryTooltipRectTransform.anchoredPosition =
            MouseCanvasPositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        tooltipTextsWithPriority.Clear();
        tooltipText.text = string.Empty;
    }
}
