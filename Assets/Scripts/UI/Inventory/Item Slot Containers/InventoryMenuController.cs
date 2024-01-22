using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
    [SerializeField] private List<GameObject> inventoryMenuGameObjects;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    [Inject] private InputManager inputManager;
    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private PauseController pauseController;
    [Inject] private PlayerActionDisablingUIManager playerActionDisablingUIManager;

    private void Awake()
    {
        this.InjectDependencies();
    }

    // Must run after any script Update methods that can set ActionDisablingUIOpen
    // to true to prevent an action disabling UI from opening on the same frame
    // that the inventory menu is opened
    private void Update()
    {
        if (playerActionDisablingUIManager.ActionDisablingUIOpen)
        {
            return;
        }

        bool openPlayerInventoryUI =
            !pauseController.GamePaused &&
            inputManager.GetInventoryButtonDownIfUnusedThisFrame();
        if (openPlayerInventoryUI)
        {
            playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

            inventoryUIManager.ShowInventoryUI(inventoryMenuGameObjects);
        }
    }
}
