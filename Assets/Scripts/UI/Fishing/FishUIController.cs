using UnityEngine;

public class FishUIController : HorizontalMover
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform fishUIImageRectTransform;
    [SerializeField] private Vector2 fishStartAbsXPositionRange;
    [SerializeField] private bool logStartPosition;

    public float Speed { private get; set; }

    private void OnEnable()
    {
        fishUIImageRectTransform.localScale = Vector3.one;

        float randomFishXPosition = Random.Range(fishStartAbsXPositionRange.x,
            fishStartAbsXPositionRange.y) * GetRandomSign();

        rectTransform.anchoredPosition = new Vector3(randomFishXPosition, 0f, 0f);

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

    private float GetRandomSign() => Random.Range(0, 2) == 0 ? 1f : -1f;
}
