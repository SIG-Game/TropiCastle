using System;
using System.Collections.Generic;
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

    private List<GameObject> currentInventoryUIGameObjects;
    private bool canCloseUsingInteractAction;
    private bool inventoryUIOpen;

    private void Awake()
    {
        inventoryUIOpen = false;
    }

    // Must run after the PauseMenu Update method so that an Escape key press
    // used to close an inventory UI isn't reused to open the pause menu
    private void Update()
    {
        if (InventoryUIOpen && CloseInventoryUIInputPressed())
        {
            DisableCurrentInventoryUI();
        }
    }

    private bool CloseInventoryUIInputPressed()
    {
        bool closeUsingEscapeKey = Input.GetKeyDown(KeyCode.Escape);

        bool closeUsingInteractAction = canCloseUsingInteractAction &&
            InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame();

        bool closeUsingInventoryAction =
            InputManager.Instance.GetInventoryButtonDownIfUnusedThisFrame();

        return closeUsingEscapeKey || closeUsingInteractAction ||
            closeUsingInventoryAction;
    }

    public void EnableCurrentInventoryUI()
    {
        SetCurrentInventoryUIActive(true);

        InventoryUIOpen = true;
        PauseController.Instance.GamePaused = true;
    }

    public void DisableCurrentInventoryUI()
    {
        SetCurrentInventoryUIActive(false);

        currentInventoryUIGameObjects = null;

        InventoryUIOpen = false;
        PauseController.Instance.GamePaused = false;
    }

    private void SetCurrentInventoryUIActive(bool active)
    {
        currentInventoryUIGameObjects.ForEach(x => x.SetActive(active));
    }

    public void SetCurrentInventoryUIGameObjects(
        List<GameObject> currentInventoryUIGameObjects)
    {
        this.currentInventoryUIGameObjects = currentInventoryUIGameObjects;
    }

    public void SetCanCloseUsingInteractAction(bool canCloseUsingInteractAction)
    {
        this.canCloseUsingInteractAction = canCloseUsingInteractAction;
    }
}
