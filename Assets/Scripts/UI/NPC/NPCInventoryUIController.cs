using System.Collections.Generic;
using UnityEngine;

public abstract class NPCInventoryUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> uiGameObjects;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    [Inject] protected InventoryUIManager inventoryUIManager;
    [Inject("PlayerInventory")] protected Inventory playerInventory;

    private void Awake()
    {
        this.InjectDependencies();
    }

    protected virtual void DisplayUI()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.ShowInventoryUI(uiGameObjects);
    }
}
