using UnityEngine;

public static class MouseCanvasPositionHelper
{
    public static Vector2 GetMouseCanvasPosition(RectTransform canvasRectTransform) =>
        ScreenPositionToCanvasPosition(Input.mousePosition, canvasRectTransform);

    private static Vector2 ScreenPositionToCanvasPosition(Vector2 screenPosition, RectTransform canvasRectTransform)
    {
        bool conversionSuccess = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, screenPosition, null, out Vector2 canvasPosition);
        if (conversionSuccess)
        {
            return canvasPosition;
        }
        else
        {
            Debug.LogError($"Failed to get {nameof(canvasPosition)} from {nameof(screenPosition)} " +
                $"using {nameof(canvasRectTransform)}");
            return Vector2.zero;
        }
    }
}
