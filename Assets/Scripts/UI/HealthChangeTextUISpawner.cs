using UnityEngine;

public class HealthChangeTextUISpawner : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private GameObject healthChangeTextPrefab;

    [Inject] private PauseController pauseController;

    private void Awake()
    {
        this.InjectDependencies();

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
        if (healthDelta != 0 && !pauseController.GamePaused)
        {
            GameObject healthChangeText =
                Instantiate(healthChangeTextPrefab, transform);
            
            healthChangeText.GetComponent<HealthChangeTextUIController>()
                .SetUpHealthChangeText(healthDelta);
        }
    }
}
