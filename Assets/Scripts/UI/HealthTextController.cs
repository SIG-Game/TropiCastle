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
