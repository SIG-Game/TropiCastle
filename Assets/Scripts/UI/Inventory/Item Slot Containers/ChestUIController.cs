using System.Collections.Generic;
using UnityEngine;

public class ChestUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> chestUIGameObjects;
    [SerializeField] private InventoryUIController chestInventoryUIController;
    [SerializeField] private InventoryUIManager inventoryUIManager;

    public static ChestUIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ShowChestUI()
    {
        inventoryUIManager.SetCurrentInventoryUIGameObjects(chestUIGameObjects);
        inventoryUIManager.SetCanCloseUsingInteractAction(true);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    public void SetChestInventory(Inventory chestInventory)
    {
        chestInventoryUIController.SetInventory(chestInventory);
    }
}
