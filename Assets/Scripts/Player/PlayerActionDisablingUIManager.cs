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

            if (actionDisablingUIOpen)
            {
                OnUIOpened();
            }
            else
            {
                OnUIClosed();
            }
        }
    }

    public event Action OnUIOpened = () => {};
    public event Action OnUIClosed = () => {};

    private bool actionDisablingUIOpen;

    private void Awake()
    {
        actionDisablingUIOpen = false;
    }
}
