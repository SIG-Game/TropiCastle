using System;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public event Action OnInventoryUIClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIManager()
    {
        InventoryUIOpen = false;
    }

    public void InvokeOnInventoryUIClosedEvent()
    {
        OnInventoryUIClosed();
    }
}
