using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int currentHealth;

    public event Action<int> OnHealthSet = delegate { };
    public event Action<int> OnHealthChangedByAmount = delegate { };
    public event Action OnHealthSetToZero = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthSet(currentHealth);
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

        OnHealthSet(currentHealth);
        OnHealthChangedByAmount(healthDelta);
    }

    public void DecreaseHealth(int amount)
    {
        int initialHealth = currentHealth;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            OnHealthSetToZero();
        }

        int healthDelta = currentHealth - initialHealth;

        OnHealthSet(currentHealth);
        OnHealthChangedByAmount(healthDelta);
    }

    public bool AtMaxHealth()
    {
        return currentHealth >= maxHealth;
    }

    public int GetCurrentHealth() => currentHealth;

    public int GetMaxHealth() => maxHealth;
}
