using System;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    private int money;

    public int Money
    {
        get => money;
        set
        {
            money = value;

            OnMoneySet();
        }
    }

    public event Action OnMoneySet = () => {};
}
