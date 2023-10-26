using System.Collections.Generic;
using UnityEngine;

public class CampfireUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> campfireUIGameObjects;
    [SerializeField] private InventoryUIController campfireInventoryUIController;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    public void Show()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(campfireUIGameObjects);
        inventoryUIManager.SetCanCloseUsingInteractAction(true);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    public void SetInventory(Inventory campfireInventory)
    {
        campfireInventoryUIController.SetInventory(campfireInventory);
    }
}
