using UnityEngine;

public class FishUIController : HorizontalMover
{
    [SerializeField] private FishingUIController fishingUIController;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform fishUIImageRectTransform;
    [SerializeField] private Vector2 fishStartAbsXPositionRange;
    [SerializeField] private bool logStartPosition;

    public float Speed { private get; set; }

    private void Awake()
    {
        fishingUIController.OnFishingUIOpened += FishingUIController_OnFishingUIOpened;
    }

    private void OnDestroy()
    {
        fishingUIController.OnFishingUIOpened -= FishingUIController_OnFishingUIOpened;
    }

    private void FishingUIController_OnFishingUIOpened()
    {
        fishUIImageRectTransform.localScale = Vector3.one;

        rectTransform.anchoredPosition = new Vector3(GetRandomFishXPosition(), 0f, 0f);

        if (logStartPosition)
        {
            Debug.Log("Set fish UI x position to: " + transform.localPosition.x);
        }

        transform.localScale = new Vector3(GetRandomSign(),
            transform.localScale.y, transform.localScale.z);

        xVelocity = Speed * transform.localScale.x;
    }

    protected override void FlipXVelocityDirection()
    {
        base.FlipXVelocityDirection();

        transform.localScale = new Vector3(-transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
    }

    private float GetRandomFishXPosition() => Random.Range(fishStartAbsXPositionRange.x,
        fishStartAbsXPositionRange.y) * GetRandomSign();

    private float GetRandomSign() => Random.Range(0, 2) == 0 ? 1f : -1f;
}
