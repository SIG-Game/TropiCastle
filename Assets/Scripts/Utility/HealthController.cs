using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int currentHealth;

    public event Action<int> OnHealthChanged = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged(currentHealth);
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnHealthChanged(currentHealth);
    }

    public void DecreaseHealth(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        OnHealthChanged(currentHealth);
    }
}
