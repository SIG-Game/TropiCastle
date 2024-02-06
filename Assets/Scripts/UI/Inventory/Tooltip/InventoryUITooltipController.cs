using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUITooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI alternateTooltipText;
    [SerializeField] private CanvasGroup tooltipBackgroundCanvasGroup;
    [SerializeField] private RectTransform tooltipBackgroundRectTransform;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private RectTransform canvasRectTransform;

    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private InventoryUIHeldItemController inventoryUIHeldItemController;

    private RectTransform rectTransform;

    private void Awake()
    {
        this.InjectDependencies();

        rectTransform = GetComponent<RectTransform>();

        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnInventoryUIClosed;
    }

    private void LateUpdate()
    {
        if (!inventoryUIManager.InventoryUIOpen)
        {
            return;
        }

        if (inventoryUIHeldItemController.HoldingItem())
        {
            SetTooltipText(inventoryUIHeldItemController);
        }
        else
        {
            UpdateTooltipTextUsingRaycast();
        }

        if (string.IsNullOrEmpty(tooltipText.text))
        {
            tooltipBackgroundCanvasGroup.alpha = 0f;
        }
        else
        {
            tooltipBackgroundCanvasGroup.alpha = 1f;

            UpdateInventoryTooltipPosition();
        }
    }

    private void OnDestroy()
    {
        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnInventoryUIClosed;
    }

    private void UpdateTooltipTextUsingRaycast()
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        var raycastResults = new List<RaycastResult>();

        graphicRaycaster.Raycast(pointerEventData, raycastResults);

        if (raycastResults.Count != 0 &&
            raycastResults[0].gameObject.TryGetComponent(
                out IElementWithTooltip elementWithTooltip))
        {
            SetTooltipText(elementWithTooltip);
        }
        else
        {
            ClearTooltipText();
        }
    }

    private void SetTooltipText(IElementWithTooltip elementWithTooltip)
    {
        SetTooltipText(elementWithTooltip.GetTooltipText(),
            elementWithTooltip.GetAlternateTooltipText());
    }

    private void SetTooltipText(
        string tooltipTextString, string alternateTooltipTextString)
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
            float distanceOverRightEdge =
                rightEdgeXPosition - canvasRectTransform.rect.xMax;

            newAnchoredPosition.x -= distanceOverRightEdge;
        }

        if (bottomEdgeYPosition < canvasRectTransform.rect.yMin)
        {
            float distanceOverBottomEdge =
                bottomEdgeYPosition - canvasRectTransform.rect.yMin;

            newAnchoredPosition.y -= distanceOverBottomEdge;
        }

        rectTransform.anchoredPosition = newAnchoredPosition;
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        ClearTooltipText();
    }
}
