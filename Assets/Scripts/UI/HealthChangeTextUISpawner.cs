using UnityEngine;

public class HealthChangeTextUISpawner : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private GameObject healthChangeTextPrefab;

    private void Awake()
    {
        targetHealthController.OnHealthChangedByAmount += HealthController_OnHealthChangedByAmount;
    }

    private void OnDestroy()
    {
        if (targetHealthController != null)
        {
            targetHealthController.OnHealthChangedByAmount -= HealthController_OnHealthChangedByAmount;
        }
    }

    private void HealthController_OnHealthChangedByAmount(int healthDelta)
    {
        if (healthDelta != 0 && !PauseController.Instance.GamePaused)
        {
            GameObject healthChangeTextGameObject = Instantiate(healthChangeTextPrefab, transform);
            HealthChangeTextUIController healthChangeTextUIController =
                healthChangeTextGameObject.GetComponent<HealthChangeTextUIController>();

            healthChangeTextUIController.SetUpHealthChangeText(healthDelta);
        }
    }
}
