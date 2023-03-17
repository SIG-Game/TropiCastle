using UnityEngine;

public class FishUIController : MonoBehaviour
{
    [SerializeField] private float fishMinX;
    [SerializeField] private float fishMaxX;
    [SerializeField] private float fishStartPositionMinAbsX;
    [SerializeField] private float fishStartPositionMaxAbsX;
    [SerializeField] private bool logStartPosition;

    private RectTransform rectTransform;

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

        bool fishMovedPastXPositionLimit = transform.localPosition.x >= fishMaxX ||
            transform.localPosition.x <= fishMinX;
        if (fishMovedPastXPositionLimit)
        {
            ClampFishXPositionToLimit();
            ChangeFishDirection();
        }
    }

    private void ClampFishXPositionToLimit()
    {
        float clampedFishXPosition = Mathf.Clamp(transform.localPosition.x, fishMinX, fishMaxX);
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

        float randomFishXDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
        transform.localScale = new Vector3(randomFishXDirection,
            transform.localScale.y, transform.localScale.z);
    }

    private float GetRandomFishXPosition()
    {
        float xPosition = Random.Range(fishStartPositionMinAbsX, fishStartPositionMaxAbsX);

        if (Random.Range(0, 2) == 0)
        {
            xPosition *= -1f;
        }

        return xPosition;
    }
}
