using UnityEngine;

public static class MouseCanvasPositionHelper
{
    public static Vector2 GetClampedMouseCanvasPosition(RectTransform canvasRectTransform)
    {
        Vector2 mouseCanvasPosition = ScreenPositionToCanvasPosition(Input.mousePosition, canvasRectTransform);
        Vector2 clampedMouseCanvasPosition = ClampPositionWithCenterAnchorToCanvas(mouseCanvasPosition, canvasRectTransform);
        return clampedMouseCanvasPosition;
    }

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

    private static Vector2 ClampPositionWithCenterAnchorToCanvas(Vector2 position, RectTransform canvasRectTransform)
    {
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        Vector2 clampedPosition = new Vector2(ClampValueWithCenterAnchorToLimit(position.x, canvasWidth),
            ClampValueWithCenterAnchorToLimit(position.y, canvasHeight));

        return clampedPosition;
    }

    private static float ClampValueWithCenterAnchorToLimit(float value, float limit)
    {
        float halfLimit = limit / 2f;
        return Mathf.Clamp(value, -halfLimit, halfLimit);
    }
}
