using UnityEngine;

public class HookUIController : HorizontalMover
{
    [SerializeField] private float xSpeed;

    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        float randomXPosition = Random.Range(xPositionRange.x, xPositionRange.y);

        transform.localPosition = new Vector3(randomXPosition, initialPosition.y, initialPosition.z);

        float randomXVelocityDirection = Random.Range(0, 2) == 0 ? 1f : -1f;

        xVelocity = xSpeed * randomXVelocityDirection;
    }
}
