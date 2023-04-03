using UnityEngine;

public class HealthChangeTextUISpawner : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private GameObject healthChangeTextPrefab;

    private int? previousHealth;

    private void Awake()
    {
        targetHealthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    private void OnDestroy()
    {
        targetHealthController.OnHealthChanged -= HealthController_OnHealthChanged;
    }

    private void HealthController_OnHealthChanged(int newHealth)
    {
        if (previousHealth != null)
        {
            GameObject healthChangeTextGameObject = Instantiate(healthChangeTextPrefab, transform);
            HealthChangeTextUIController healthChangeTextUIController =
                healthChangeTextGameObject.GetComponent<HealthChangeTextUIController>();

            int healthDelta = newHealth - previousHealth.Value;

            healthChangeTextUIController.SetUpHealthChangeText(healthDelta);
        }

        previousHealth = newHealth;
    }
}
