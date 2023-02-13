using TMPro;
using UnityEngine;

public class IngredientsTooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform canvasRectTransform;

    private RectTransform ingredientsTooltipRectTransform;
    private bool tooltipIsVisible;

    private void Awake()
    {
        ingredientsTooltipRectTransform = GetComponent<RectTransform>();
        tooltipIsVisible = false;
    }

    private void Update()
    {
        if (tooltipIsVisible)
        {
            UpdateIngredientsTooltipPosition();
        }
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
        bool mousePositionConvertedToIngredientsTooltipAnchoredPosition = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, Input.mousePosition, null, out Vector2 ingredientsTooltipAnchoredPosition);
        if (mousePositionConvertedToIngredientsTooltipAnchoredPosition)
        {
            ingredientsTooltipRectTransform.anchoredPosition = ingredientsTooltipAnchoredPosition;
        }
        else
        {
            Debug.LogError($"Failed to get {nameof(ingredientsTooltipAnchoredPosition)} from {nameof(Input.mousePosition)} " +
                $"using {nameof(canvasRectTransform)}");
        }
    }
}
