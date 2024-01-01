using UnityEngine;

public class HealthBarSpriteController : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private Transform fill;
    [SerializeField] private float interpolationSpeed;

    private float maxHealthFloat;
    private float currentFillXPosition;
    private float targetFillXPosition;
    private bool initialHealthSet;

    private const float minFillXPosition = -1f;

    private void Awake()
    {
        maxHealthFloat = healthController.MaxHealth;
        currentFillXPosition = fill.localPosition.x;
        targetFillXPosition = currentFillXPosition;
        initialHealthSet = false;

        healthController.OnHealthSet += HealthController_OnHealthSet;
    }

    private void OnDestroy()
    {
        healthController.OnHealthSet -= HealthController_OnHealthSet;
    }

    private void Update()
    {
        if (currentFillXPosition != targetFillXPosition)
        {
            currentFillXPosition = Mathf.MoveTowards(currentFillXPosition,
                targetFillXPosition, interpolationSpeed * Time.unscaledDeltaTime);

            fill.localPosition = new Vector3(currentFillXPosition,
                fill.localPosition.y, fill.localPosition.z);
        }
    }

    private void HealthController_OnHealthSet(int newHealth)
    {
        float currentHealthNormalized = newHealth / maxHealthFloat;

        targetFillXPosition = minFillXPosition + currentHealthNormalized;

        if (!initialHealthSet)
        {
            currentFillXPosition = targetFillXPosition;

            fill.localPosition = new Vector3(currentFillXPosition,
                fill.localPosition.y, fill.localPosition.z);

            initialHealthSet = true;
        }
    }
}
