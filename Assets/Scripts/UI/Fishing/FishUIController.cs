using UnityEngine;

public class FishUIController : HorizontalMover
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform fishUIImageRectTransform;
    [SerializeField] private Vector2 fishStartAbsXPositionRange;
    [SerializeField] private bool logStartPosition;

    public float Speed { private get; set; }

    public void SetRandomXPosition()
    {
        rectTransform.anchoredPosition = new Vector3(GetRandomFishXPosition(), 0f, 0f);

        if (logStartPosition)
        {
            Debug.Log("Set fish UI x position to: " + transform.localPosition.x);
        }
    }

    public void SetRandomXDirection()
    {
        float randomFishXDirection = GetRandomSign();
        transform.localScale = new Vector3(randomFishXDirection,
            transform.localScale.y, transform.localScale.z);
    }

    public void UpdateXVelocity()
    {
        xVelocity = Speed * transform.localScale.x;
    }

    public void ResetFishUIImage()
    {
        fishUIImageRectTransform.localScale = Vector3.one;
    }

    private float GetRandomFishXPosition() => Random.Range(fishStartAbsXPositionRange.x,
        fishStartAbsXPositionRange.y) * GetRandomSign();

    private float GetRandomSign() => Random.Range(0, 2) == 0 ? 1f : -1f;

    protected override void FlipXVelocityDirection()
    {
        base.FlipXVelocityDirection();

        transform.localScale = new Vector3(-transform.localScale.x,
            transform.localScale.y, transform.localScale.z);
    }
}
