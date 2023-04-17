using UnityEngine;

public class HealthBarUIController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private RectTransform healthBarFill;

    private float targetMaxHealthFloat;

    private void Awake()
    {
        targetMaxHealthFloat = targetHealthController.GetMaxHealth();

        targetHealthController.OnHealthChanged += HealthController_OnHealthChanged;
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
        float normalizedFillAmount = newHealth / targetMaxHealthFloat;

        healthBarFill.localScale = new Vector3(normalizedFillAmount,
            healthBarFill.localScale.y, healthBarFill.localScale.z);
    }
}
