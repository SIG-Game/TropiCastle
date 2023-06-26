using UnityEngine;

public class HealthBarUIController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private RectTransform healthBarFill;
    [SerializeField] private float fillChangeSpeed;

    private float targetMaxHealthFloat;
    private float currentFillXScale;
    private float targetFillXScale;
    private bool initialHealthSet;

    private void Awake()
    {
        targetMaxHealthFloat = targetHealthController.GetMaxHealth();

        currentFillXScale = 1f;
        targetFillXScale = 1f;
        initialHealthSet = false;

        targetHealthController.OnHealthSet += HealthController_OnHealthSet;
    }

    private void Update()
    {
        if (currentFillXScale != targetFillXScale)
        {
            // unscaledDeltaTime is used so that interpolation can occur while the game is paused
            currentFillXScale = Mathf.MoveTowards(currentFillXScale, targetFillXScale,
                fillChangeSpeed * Time.unscaledDeltaTime);

            SetHealthBarFillXScale(currentFillXScale);
        }
    }

    private void OnDestroy()
    {
        if (targetHealthController != null)
        {
            targetHealthController.OnHealthSet -= HealthController_OnHealthSet;
        }
    }

    private void SetHealthBarFillXScale(float xScale)
    {
        healthBarFill.localScale = new Vector3(xScale,
            healthBarFill.localScale.y, healthBarFill.localScale.z);
    }

    private void HealthController_OnHealthSet(int newHealth)
    {
        if (!initialHealthSet)
        {
            currentFillXScale = newHealth / targetMaxHealthFloat;

            SetHealthBarFillXScale(currentFillXScale);

            initialHealthSet = true;
        }

        targetFillXScale = newHealth / targetMaxHealthFloat;
    }
}
