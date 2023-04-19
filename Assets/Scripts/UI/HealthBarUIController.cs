using UnityEngine;

public class HealthBarUIController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private RectTransform healthBarFill;
    [SerializeField] private float fillChangeSpeed;

    private float targetMaxHealthFloat;
    private float currentFillXScale;
    private float targetFillXScale;

    private void Awake()
    {
        targetMaxHealthFloat = targetHealthController.GetMaxHealth();

        currentFillXScale = 1f;
        targetFillXScale = 1f;

        targetHealthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    private void Update()
    {
        if (currentFillXScale != targetFillXScale)
        {
            // unscaledDeltaTime is used so that interpolation can occur while the game is paused
            currentFillXScale = Mathf.MoveTowards(currentFillXScale, targetFillXScale,
                fillChangeSpeed * Time.unscaledDeltaTime);

            healthBarFill.localScale = new Vector3(currentFillXScale,
                healthBarFill.localScale.y, healthBarFill.localScale.z);
        }
    }

    private void OnDestroy()
    {
        if (targetHealthController != null)
        {
            targetHealthController.OnHealthChanged -= HealthController_OnHealthChanged;
        }
    }

    private void HealthController_OnHealthChanged(int newHealth, int _)
    {
        targetFillXScale = newHealth / targetMaxHealthFloat;
    }
}
