using System;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public bool InventoryUIOpen
    {
        get => inventoryUIOpen;
        set
        {
            inventoryUIOpen = value;

            if (!inventoryUIOpen)
            {
                OnInventoryUIClosed();
            }
        }
    }

    public event Action OnInventoryUIClosed = delegate { };

    private bool inventoryUIOpen;

    private void Awake()
    {
        inventoryUIOpen = false;
    }
}
