using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour
{
    [SerializeField] private List<GameObject> inventoryMenuGameObjects;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Vector2 playerInventoryUIPosition;

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
            !PauseController.Instance.GamePaused &&
            inputManager.GetInventoryButtonDownIfUnusedThisFrame();
        if (openPlayerInventoryUI)
        {
            playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

            inventoryUIManager.SetCurrentInventoryUIGameObjects(inventoryMenuGameObjects);
            inventoryUIManager.SetCanCloseUsingInteractAction(false);
            inventoryUIManager.EnableCurrentInventoryUI();
        }
    }
}
