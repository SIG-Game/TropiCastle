using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class InventoryUITooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI alternateTooltipText;
    [SerializeField] private RectTransform tooltipBackgroundRectTransform;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private bool logTooltipList;

    private List<Tooltip> tooltipTextsWithPriority;

    private RectTransform rectTransform;

    public static InventoryUITooltipController Instance;

    private void Awake()
    {
        Instance = this;

        tooltipTextsWithPriority = new List<Tooltip>();

        rectTransform = GetComponent<RectTransform>();

        inventoryUIManager.OnInventoryUIClosed += InventoryUIManager_OnInventoryUIClosed;
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

        inventoryUIManager.OnInventoryUIClosed -= InventoryUIManager_OnInventoryUIClosed;
    }

    public void AddTooltipTextWithPriority(Tooltip textWithPriority)
    {
        tooltipTextsWithPriority.Add(textWithPriority);

        LogTooltipListAfterTooltipOperation("adding");

        UpdateTooltipText();
        UpdateInventoryTooltipPosition();
    }

    public void RemoveTooltipTextWithPriority(Tooltip textWithPriority)
    {
        tooltipTextsWithPriority.Remove(textWithPriority);

        LogTooltipListAfterTooltipOperation("removing");

        UpdateTooltipText();
        UpdateInventoryTooltipPosition();
    }

    private void UpdateTooltipText()
    {
        string tooltipTextWithHighestPriority = string.Empty;
        string alternateTooltipTextWithHighestPriority = string.Empty;

        if (tooltipTextsWithPriority.Count > 0)
        {
            var tooltipWithHighestPriority = tooltipTextsWithPriority.Aggregate((max, next) =>
                next.Priority > max.Priority ? next : max);
            tooltipTextWithHighestPriority = tooltipWithHighestPriority.Text;

            if (tooltipWithHighestPriority.AlternateText != null)
            {
                alternateTooltipTextWithHighestPriority =
                    tooltipWithHighestPriority.AlternateText;
            }
        }

        tooltipText.text = tooltipTextWithHighestPriority;
        alternateTooltipText.text = alternateTooltipTextWithHighestPriority;

        // Refresh tooltipText and alternateTooltipText preferred sizes
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipBackgroundRectTransform);
    }

    private void UpdateInventoryTooltipPosition()
    {
        Vector2 newAnchoredPosition =
            MousePositionHelper.GetClampedMouseCanvasPosition(canvasRectTransform);

        float rightEdgeXPosition = newAnchoredPosition.x +
            tooltipBackgroundRectTransform.rect.xMax;
        float bottomEdgeYPosition = newAnchoredPosition.y +
            tooltipBackgroundRectTransform.rect.yMin;

        if (rightEdgeXPosition > canvasRectTransform.rect.xMax)
        {
            float distanceOverRightEdge = rightEdgeXPosition - canvasRectTransform.rect.xMax;

            newAnchoredPosition.x -= distanceOverRightEdge;
        }

        if (bottomEdgeYPosition < canvasRectTransform.rect.yMin)
        {
            float distanceOverBottomEdge = bottomEdgeYPosition - canvasRectTransform.rect.yMin;

            newAnchoredPosition.y -= distanceOverBottomEdge;
        }

        rectTransform.anchoredPosition = newAnchoredPosition;
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        tooltipTextsWithPriority.Clear();
        tooltipText.text = string.Empty;
        alternateTooltipText.text = string.Empty;
    }

    private void LogTooltipList()
    {
        var tooltipListStringBuilder = new StringBuilder();

        foreach (Tooltip textWithPriority in tooltipTextsWithPriority)
        {
            string textWithoutNewlines = textWithPriority.Text.Replace("\n", string.Empty);
            tooltipListStringBuilder.Append($"\"{textWithoutNewlines}\": {textWithPriority.Priority}, ");
        }

        Debug.Log(tooltipListStringBuilder.ToString());
    }

    private void LogTooltipListAfterTooltipOperation(string operationName)
    {
        if (logTooltipList)
        {
            // callingMethod is the method that called the method
            // that called LogTooltipListAfterTooltipOperation
            MethodBase callingMethod = new StackFrame(2).GetMethod();

            string callingMethodName = callingMethod.Name;
            string callingType = callingMethod.DeclaringType.Name;

            Debug.Log($"Tooltip list after {operationName} called from " +
                $"{callingMethodName} in {callingType}:");

            LogTooltipList();
        }
    }

    public bool TooltipListContains(Tooltip textWithPriority) =>
        tooltipTextsWithPriority.Contains(textWithPriority);
}
