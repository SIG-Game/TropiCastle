using TMPro;
using UnityEngine;

public class HealthTextController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private bool displayHealthPrefix;

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
        if (displayHealthPrefix)
        {
            healthText.text = "Health: " + newHealth;
        }
        else
        {
            healthText.text = newHealth.ToString();
        }
    }
}
