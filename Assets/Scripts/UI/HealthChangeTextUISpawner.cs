using UnityEngine;

public class HealthChangeTextUISpawner : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private GameObject healthChangeTextPrefab;

    private void Awake()
    {
        targetHealthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    private void OnDestroy()
    {
        if (targetHealthController != null)
        {
            targetHealthController.OnHealthChanged -= HealthController_OnHealthChanged;
        }
    }

    private void HealthController_OnHealthChanged(int newHealth, int healthDelta)
    {
        if (healthDelta != 0)
        {
            GameObject healthChangeTextGameObject = Instantiate(healthChangeTextPrefab, transform);
            HealthChangeTextUIController healthChangeTextUIController =
                healthChangeTextGameObject.GetComponent<HealthChangeTextUIController>();

            healthChangeTextUIController.SetUpHealthChangeText(healthDelta);
        }
    }
}
