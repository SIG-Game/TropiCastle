using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuBackground;

    [Inject] private InputManager inputManager;
    [Inject] private PauseController pauseController;

    public bool InventoryUIOpen
    {
        get => inventoryUIOpen;
        set
        {
            inventoryUIOpen = value;

            pauseController.GamePaused = inventoryUIOpen;

            if (!inventoryUIOpen)
            {
                OnInventoryUIClosed();
            }
        }
    }

    public event Action OnInventoryUIClosed = () => {};

    private List<GameObject> currentInventoryUIGameObjects;
    private bool inventoryUIOpen;

    private void Awake()
    {
        this.InjectDependencies();
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

        bool closeUsingInteractAction =
            inputManager.GetInteractButtonDownIfUnusedThisFrame();

        bool closeUsingInventoryAction =
            inputManager.GetInventoryButtonDownIfUnusedThisFrame();

        return closeUsingEscapeKey || closeUsingInteractAction ||
            closeUsingInventoryAction;
    }

    public void ShowInventoryUI(List<GameObject> inventoryUIGameObjects)
    {
        currentInventoryUIGameObjects = inventoryUIGameObjects;

        SetCurrentInventoryUIActive(true);

        InventoryUIOpen = true;
    }

    public void DisableCurrentInventoryUI()
    {
        SetCurrentInventoryUIActive(false);

        currentInventoryUIGameObjects = null;

        InventoryUIOpen = false;
    }

    private void SetCurrentInventoryUIActive(bool active)
    {
        currentInventoryUIGameObjects.ForEach(x => x.SetActive(active));

        menuBackground.SetActive(active);
    }
}
