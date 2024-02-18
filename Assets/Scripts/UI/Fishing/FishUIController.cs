using UnityEngine;

public class FishUIController : HorizontalMover
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform fishUIImageRectTransform;
    [SerializeField] private Vector2 fishStartAbsXPositionRange;

    public float Speed { private get; set; }

    public override void StartMovement()
    {
        fishUIImageRectTransform.localScale = Vector3.one;

        float randomFishXPosition = Random.Range(fishStartAbsXPositionRange.x,
            fishStartAbsXPositionRange.y) * GetRandomSign();

        rectTransform.anchoredPosition = new Vector2(
            randomFishXPosition,
            rectTransform.anchoredPosition.y);

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

    private float GetRandomSign() => Random.Range(0, 2) == 0 ? 1f : -1f;
}
