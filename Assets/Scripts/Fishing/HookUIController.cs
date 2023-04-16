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
        transform.localPosition = initialPosition;

        xVelocity = xSpeed;
    }

    private void Update()
    {
        Vector3 newPosition = transform.localPosition + Vector3.right * xVelocity * Time.deltaTime;

        bool hookMovedPastXPositionLimit = newPosition.x <= hookXPositionRange.x ||
            newPosition.x >= hookXPositionRange.y;
        if (hookMovedPastXPositionLimit)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, hookXPositionRange.x, hookXPositionRange.y);

            xVelocity *= -1f;
        }

        transform.localPosition = newPosition;
    }
}
