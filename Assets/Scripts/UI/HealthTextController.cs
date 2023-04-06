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
