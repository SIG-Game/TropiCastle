using UnityEngine;
using UnityEngine.UI;

public class AutoHealButton : MonoBehaviour
{
    [SerializeField] private Button autoHealButton;
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private HealingItemUsage healingItemUsage;

    private void Awake()
    {
        playerHealthController.OnHealthSet += HealthController_OnHealthSet;
    }

    private void OnDestroy()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnHealthSet -= HealthController_OnHealthSet;
        }
    }

    public void AutoHealButton_OnClick()
    {
        healingItemUsage.UseHealingItemsUntilMaxHealthReached();
    }

    private void HealthController_OnHealthSet(int _)
    {
        autoHealButton.interactable = !playerHealthController.AtMaxHealth();
    }
}
