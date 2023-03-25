using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class InventoryUITooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private bool logTooltipList;

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

        if (logTooltipList)
        {
            MethodBase callingMethod = new StackFrame(1).GetMethod();
            string callingMethodName = callingMethod.Name;
            string callingType = callingMethod.DeclaringType.Name;
            Debug.Log($"After adding called from {callingMethodName} in {callingType}:");
            LogTooltipList();
        }

        tooltipText.text = GetTooltipTextWithHighestPriority();
        UpdateInventoryTooltipPosition();
    }

    public void RemoveTooltipTextWithPriority(KeyValuePair<string, int> textWithPriority)
    {
        tooltipTextsWithPriority.Remove(textWithPriority);

        if (logTooltipList)
        {
            MethodBase callingMethod = new StackFrame(1).GetMethod();
            string callingMethodName = callingMethod.Name;
            string callingType = callingMethod.DeclaringType.Name;
            Debug.Log($"After removing called from {callingMethodName} in {callingType}:");
            LogTooltipList();
        }

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

    private void LogTooltipList()
    {
        var tooltipListStringBuilder = new StringBuilder();

        foreach (KeyValuePair<string, int> textWithPriority in tooltipTextsWithPriority)
        {
            string textWithoutNewlines = textWithPriority.Key.Replace("\n", string.Empty);
            tooltipListStringBuilder.Append($"\"{textWithoutNewlines}\": {textWithPriority.Value}, ");
        }

        Debug.Log(tooltipListStringBuilder.ToString());
    }
}
