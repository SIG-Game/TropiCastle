using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUITooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI alternateTooltipText;
    [SerializeField] private RectTransform tooltipBackgroundRectTransform;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool logTooltipList;

    private RectTransform rectTransform;

    public static InventoryUITooltipController Instance;

    private void Awake()
    {
        Instance = this;

        rectTransform = GetComponent<RectTransform>();

        inventoryUIManager.OnInventoryUIClosed += InventoryUIManager_OnInventoryUIClosed;
    }

    private void LateUpdate()
    {
        if (!inventoryUIManager.InventoryUIOpen)
        {
            return;
        }

        if (InventoryUIHeldItemController.Instance.HoldingItem())
        {
            SetTooltipText(InventoryUIHeldItemController.Instance);
        }
        else
        {
            UpdateTooltipTextUsingRaycast();
        }

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

    private void UpdateTooltipTextUsingRaycast()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        graphicRaycaster.Raycast(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.TryGetComponent(out IElementWithTooltip elementWithTooltip))
            {
                SetTooltipText(elementWithTooltip);

                break;
            }
            else
            {
                ClearTooltipText();
            }
        }

        if (raycastResults.Count == 0)
        {
            ClearTooltipText();
        }
    }

    private void SetTooltipText(IElementWithTooltip elementWithTooltip)
    {
        if (elementWithTooltip is
            IElementWithMultiTextTooltip elementWithMultiTextTooltip)
        {
            SetTooltipText(elementWithMultiTextTooltip.GetTooltipText(),
                elementWithMultiTextTooltip.GetAlternateTooltipText());
        }
        else
        {
            SetTooltipText(elementWithTooltip.GetTooltipText(), string.Empty);
        }
    }

    private void SetTooltipText(string tooltipTextString, string alternateTooltipTextString)
    {
        tooltipText.text = tooltipTextString;
        alternateTooltipText.text = alternateTooltipTextString;

        // Refresh tooltipText and alternateTooltipText preferred sizes
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipBackgroundRectTransform);
    }

    private void ClearTooltipText()
    {
        SetTooltipText(string.Empty, string.Empty);
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
        ClearTooltipText();
    }
}
