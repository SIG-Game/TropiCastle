using TMPro;
using UnityEngine;

public class HealthTextController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;

    private TextMeshProUGUI healthText;

    private void Awake()
    {
        healthText = GetComponent<TextMeshProUGUI>();

        targetHealthController.OnHealthChanged += HealthController_OnHeathChanged;
    }

    private void OnDestroy()
    {
        targetHealthController.OnHealthChanged -= HealthController_OnHeathChanged;
    }

    private void HealthController_OnHeathChanged(int newHealth)
    {
        healthText.text = "Health: " + newHealth;
    }
}
