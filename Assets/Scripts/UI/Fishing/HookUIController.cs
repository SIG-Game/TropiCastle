using UnityEngine;

public class HookUIController : MonoBehaviour
{
    [SerializeField] private Vector2 hookXPositionRange;
    [SerializeField] private float xSpeed;

    private Vector3 initialPosition;
    private float xVelocity;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        float randomXPosition = Random.Range(hookXPositionRange.x, hookXPositionRange.y);

        transform.localPosition = new Vector3(randomXPosition, initialPosition.y, initialPosition.z);

        float randomXVelocityDirection = Random.Range(0, 2) == 0 ? 1f : -1f;

        xVelocity = xSpeed * randomXVelocityDirection;
    }

    private void Update()
    {
        Vector3 newPosition = transform.localPosition + Vector3.right * xVelocity * Time.deltaTime;

        if (newPosition.x <= hookXPositionRange.x)
        {
            float distanceOverLimit = hookXPositionRange.x - newPosition.x;

            newPosition.x = hookXPositionRange.x + distanceOverLimit;

            xVelocity *= -1f;
        }
        else if (newPosition.x >= hookXPositionRange.y)
        {
            float distanceOverLimit = newPosition.x - hookXPositionRange.y;

            newPosition.x = hookXPositionRange.y - distanceOverLimit;

            xVelocity *= -1f;
        }

        transform.localPosition = newPosition;
    }
}
