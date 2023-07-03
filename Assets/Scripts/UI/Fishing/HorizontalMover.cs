using UnityEngine;

public abstract class HorizontalMover : MonoBehaviour
{
    [SerializeField] protected Vector2 xPositionRange;

    protected float xVelocity;

    private void Update()
    {
        Vector3 newPosition = transform.localPosition +
            xVelocity * Time.deltaTime * Vector3.right;

        if (newPosition.x <= xPositionRange.x)
        {
            float distanceOverLimit = xPositionRange.x - newPosition.x;

            newPosition.x = xPositionRange.x + distanceOverLimit;

            FlipXVelocityDirection();
        }
        else if (newPosition.x >= xPositionRange.y)
        {
            float distanceOverLimit = newPosition.x - xPositionRange.y;

            newPosition.x = xPositionRange.y - distanceOverLimit;

            FlipXVelocityDirection();
        }

        transform.localPosition = newPosition;
    }

    protected virtual void FlipXVelocityDirection()
    {
        xVelocity *= -1f;
    }
}
