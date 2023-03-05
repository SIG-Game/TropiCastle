using TMPro;
using UnityEngine;

public class HealthTextController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;

    private TMP_Text healthText;

    private void Awake()
    {
        healthText = GetComponent<TMP_Text>();

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
