using System;
using UnityEngine;

public class PlayerActionDisablingUIManager : MonoBehaviour
{
    // Not used for pausing menus
    public bool ActionDisablingUIOpen
    {
        get => actionDisablingUIOpen;
        set
        {
            actionDisablingUIOpen = value;
            OnActionDisablingUIOpenSet();
        }
    }

    public event Action OnActionDisablingUIOpenSet = delegate { };

    private bool actionDisablingUIOpen;

    private void Awake()
    {
        actionDisablingUIOpen = false;
    }
}
