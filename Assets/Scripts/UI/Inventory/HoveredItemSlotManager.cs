using UnityEngine;

public class HoveredItemSlotManager : MonoBehaviour
{
    [SerializeField] private InventoryUIManager inventoryUIManager;

    public int HoveredItemIndex { get; set; }
    public Inventory HoveredInventory { get; set; }

    private void Awake()
    {
        HoveredItemIndex = -1;

        inventoryUIManager.OnInventoryUIClosed += InventoryUIManager_OnInventoryUIClosed;
    }

    private void OnDestroy()
    {
        inventoryUIManager.OnInventoryUIClosed -= InventoryUIManager_OnInventoryUIClosed;
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        HoveredItemIndex = -1;
    }
}
