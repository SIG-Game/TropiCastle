using System.Collections.Generic;
using UnityEngine;

public class ChestUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> chestUIGameObjects;
    [SerializeField] private InventoryUIController chestInventoryUIController;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    public void ShowChestUI()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(chestUIGameObjects);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    public void SetChestInventory(Inventory chestInventory)
    {
        chestInventoryUIController.SetInventory(chestInventory);
    }
}
