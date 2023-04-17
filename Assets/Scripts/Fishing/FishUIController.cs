using UnityEngine;
using UnityEngine.UI;

public class FishUIController : MonoBehaviour
{
    [SerializeField] private Image fishUIImage;
    [SerializeField] private Vector2 fishXPositionRange;
    [SerializeField] private Vector2 fishStartAbsXPositionRange;
    [SerializeField] private bool logStartPosition;

    private RectTransform rectTransform;
    private RectTransform fishUIImageRectTransform;

    public float Speed { private get; set; }

    private void Update()
    {
        UpdateFishPosition();
    }

    private void UpdateFishPosition()
    {
        float fishXDirection = transform.localScale.x;
        float fishXSpeed = Speed * Time.deltaTime;
        transform.localPosition += Vector3.right * fishXDirection * fishXSpeed;

        bool fishMovedPastXPositionLimit = transform.localPosition.x <= fishXPositionRange.x ||
            transform.localPosition.x >= fishXPositionRange.y;
        if (fishMovedPastXPositionLimit)
        {
            ClampFishXPositionToLimit();
            ChangeFishDirection();
        }
    }

    private void ClampFishXPositionToLimit()
    {
        float clampedFishXPosition = Mathf.Clamp(transform.localPosition.x,
            fishXPositionRange.x, fishXPositionRange.y);
        transform.localPosition = new Vector3(clampedFishXPosition,
                transform.localPosition.y, transform.localPosition.z);
    }

    private void ChangeFishDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
    }

    public void SetFishUIPositionAndDirection()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        rectTransform.anchoredPosition = new Vector3(GetRandomFishXPosition(), 0f, 0f);

        if (logStartPosition)
        {
            Debug.Log("Set fish UI x position to: " + transform.localPosition.x);
        }

        float randomFishXDirection = GetRandomSign();
        transform.localScale = new Vector3(randomFishXDirection,
            transform.localScale.y, transform.localScale.z);
    }

    public void ResetFishUIImage()
    {
        if (fishUIImageRectTransform == null)
        {
            fishUIImageRectTransform = fishUIImage.GetComponent<RectTransform>();
        }

        fishUIImageRectTransform.localScale = Vector3.one;

        fishUIImage.color = new Color(fishUIImage.color.r,
            fishUIImage.color.g, fishUIImage.color.b, 1f);
    }

    private float GetRandomFishXPosition() => Random.Range(fishStartAbsXPositionRange.x,
        fishStartAbsXPositionRange.y) * GetRandomSign();

    private float GetRandomSign() => Random.Range(0, 2) == 0 ? 1f : -1f;

    public void SetColor(Color color)
    {
        fishUIImage.color = color;
    }
}
