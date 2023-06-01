using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int currentHealth;

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;

            OnHealthSet(currentHealth);

            if (currentHealth == 0)
            {
                OnHealthSetToZero();
            }
        }
    }

    public event Action<int> OnHealthSet = delegate { };
    public event Action<int> OnHealthChangedByAmount = delegate { };
    public event Action OnHealthSetToZero = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // currentHealth can be set by another script between the execution of Awake
    // and Start, and OnHealthSet should not be raised in Awake because other
    // scripts may have not yet subscribed to that event at that time
    private void Start()
    {
        OnHealthSet(currentHealth);
    }

    public void IncreaseHealth(int amount)
    {
        int newHealth = CurrentHealth + amount;

        if (newHealth > maxHealth)
        {
            newHealth = maxHealth;
        }

        int healthDelta = newHealth - CurrentHealth;

        CurrentHealth = newHealth;

        OnHealthChangedByAmount(healthDelta);
    }

    public void DecreaseHealth(int amount)
    {
        int newHealth = CurrentHealth - amount;

        if (newHealth <= 0)
        {
            newHealth = 0;
        }

        int healthDelta = newHealth - CurrentHealth;

        CurrentHealth = newHealth;

        OnHealthChangedByAmount(healthDelta);
    }

    public bool AtMaxHealth() => CurrentHealth >= maxHealth;

    public int GetMaxHealth() => maxHealth;
}
