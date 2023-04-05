using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int currentHealth;

    // Event parameters are newHealth and healthDelta
    public event Action<int, int> OnHealthChanged = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged(currentHealth, 0);
    }

    public void IncreaseHealth(int amount)
    {
        int initialHealth = currentHealth;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        int healthDelta = currentHealth - initialHealth;

        OnHealthChanged(currentHealth, healthDelta);
    }

    public void DecreaseHealth(int amount)
    {
        int initialHealth = currentHealth;

        currentHealth -= amount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        int healthDelta = currentHealth - initialHealth;

        OnHealthChanged(currentHealth, healthDelta);
    }

    public bool AtMaxHealth()
    {
        return currentHealth >= maxHealth;
    }
}
