using System.Text;
using TMPro;
using UnityEngine;

public class HealthTextController : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private bool displayHealthPrefix;
    [SerializeField] private bool displayMaxHealth;

    private TMP_Text healthText;
    private int targetMaxHealth;

    private void Awake()
    {
        healthText = GetComponent<TMP_Text>();

        if (displayMaxHealth)
        {
            targetMaxHealth = targetHealthController.MaxHealth;
        }

        targetHealthController.OnHealthSet += HealthController_OnHealthSet;
    }

    private void OnDestroy()
    {
        if (targetHealthController != null)
        {
            targetHealthController.OnHealthSet -= HealthController_OnHealthSet;
        }
    }

    private void HealthController_OnHealthSet(int newHealth)
    {
        StringBuilder healthTextStringBuilder = new StringBuilder();

        if (displayHealthPrefix)
        {
            healthTextStringBuilder.Append("Health: ");
        }

        healthTextStringBuilder.Append(newHealth);

        if (displayMaxHealth)
        {
            healthTextStringBuilder.Append($" / {targetMaxHealth}");
        }

        healthText.text = healthTextStringBuilder.ToString();
    }
}
