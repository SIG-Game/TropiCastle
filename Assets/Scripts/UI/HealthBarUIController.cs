using UnityEngine;

public class HealthBarUIController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private RectTransform healthBarFill;
    [SerializeField] private float fillChangeSpeed;

    private float targetMaxHealthFloat;
    private float fillWidth;
    private float minHealthFillXPosition;
    private float currentFillXPosition;
    private float targetFillXPosition;
    private bool initialHealthSet;

    private void Awake()
    {
        targetMaxHealthFloat = targetHealthController.MaxHealth;
        fillWidth = healthBarFill.sizeDelta.x;
        minHealthFillXPosition =
            healthBarFill.anchoredPosition.x - fillWidth;

        currentFillXPosition = healthBarFill.anchoredPosition.x;
        targetFillXPosition = currentFillXPosition;
        initialHealthSet = false;

        targetHealthController.OnHealthSet += HealthController_OnHealthSet;
    }

    private void Update()
    {
        if (currentFillXPosition != targetFillXPosition)
        {
            // unscaledDeltaTime is used so that interpolation can occur while the game is paused
            currentFillXPosition = Mathf.MoveTowards(currentFillXPosition, targetFillXPosition,
                fillChangeSpeed * Time.unscaledDeltaTime);

            SetHealthBarFillXPosition(currentFillXPosition);
        }
    }

    private void OnDestroy()
    {
        if (targetHealthController != null)
        {
            targetHealthController.OnHealthSet -= HealthController_OnHealthSet;
        }
    }

    private void SetHealthBarFillXPosition(float xPosition)
    {
        healthBarFill.anchoredPosition =
            new Vector3(xPosition, healthBarFill.anchoredPosition.y);
    }

    private void HealthController_OnHealthSet(int newHealth)
    {
        float currentHealthNormalized =
            newHealth / targetMaxHealthFloat;

        targetFillXPosition = minHealthFillXPosition +
            fillWidth * currentHealthNormalized;

        if (!initialHealthSet)
        {
            currentFillXPosition = targetFillXPosition;

            SetHealthBarFillXPosition(currentFillXPosition);

            initialHealthSet = true;
        }
    }
}
