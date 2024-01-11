using System.Collections.Generic;
using UnityEngine;

public class ChestUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> chestUIGameObjects;
    [SerializeField] private InventoryUIController chestInventoryUIController;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    [Inject] private InventoryUIManager inventoryUIManager;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void ShowChestUI(Inventory chestInventory)
    {
        chestInventoryUIController.SetInventory(chestInventory);

        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(chestUIGameObjects);
        inventoryUIManager.EnableCurrentInventoryUI();
    }
}
