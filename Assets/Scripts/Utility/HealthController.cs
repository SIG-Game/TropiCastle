using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    public int Health
    {
        get => health;
        set
        {
            int previousHealth = health;

            health = Math.Clamp(value, 0, maxHealth);

            InvokeOnHealthSetEvents();

            if (raiseHealthChangedEvent)
            {
                OnHealthChangedByAmount(health - previousHealth);
            }
        }
    }

    public int MaxHealth => maxHealth;
    public bool AtMaxHealth => Health == MaxHealth;

    public event Action<int> OnHealthSet = (_) => {};
    public event Action<int> OnHealthChangedByAmount = (_) => {};
    public event Action OnHealthSetToZero = () => {};

    private int health;
    private bool raiseHealthChangedEvent;

    private void Awake()
    {
        health = maxHealth;

        raiseHealthChangedEvent = true;
    }

    // health can be set by another script between Awake and Start, and
    // events should not be raised in Awake because other scripts may
    // have not yet subscribed to those events at that time
    private void Start()
    {
        InvokeOnHealthSetEvents();
    }

    public void SetInitialHealth(int initialHealth)
    {
        raiseHealthChangedEvent = false;

        Health = initialHealth;

        raiseHealthChangedEvent = true;
    }

    private void InvokeOnHealthSetEvents()
    {
        OnHealthSet(Health);

        if (Health == 0)
        {
            OnHealthSetToZero();
        }
    }
}
