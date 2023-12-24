using UnityEngine;

public class HealthChangeTextUISpawner : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private GameObject healthChangeTextPrefab;

    private void Awake()
    {
        healthController.OnHealthChangedByAmount +=
            HealthController_OnHealthChangedByAmount;
    }

    private void OnDestroy()
    {
        healthController.OnHealthChangedByAmount -=
            HealthController_OnHealthChangedByAmount;
    }

    private void HealthController_OnHealthChangedByAmount(int healthDelta)
    {
        if (healthDelta != 0 && !PauseController.Instance.GamePaused)
        {
            GameObject healthChangeText =
                Instantiate(healthChangeTextPrefab, transform);
            
            healthChangeText.GetComponent<HealthChangeTextUIController>()
                .SetUpHealthChangeText(healthDelta);
        }
    }
}
