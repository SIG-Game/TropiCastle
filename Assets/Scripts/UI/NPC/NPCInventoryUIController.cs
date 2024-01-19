using System.Collections.Generic;
using UnityEngine;

public abstract class NPCInventoryUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> uiGameObjects;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] protected Inventory playerInventory;
    [SerializeField] private Vector2 playerInventoryUIPosition;

    [Inject] protected InventoryUIHeldItemController inventoryUIHeldItemController;
    [Inject] protected InventoryUIManager inventoryUIManager;

    protected virtual void Awake()
    {
        this.InjectDependencies();

        inventoryUIHeldItemController.OnItemHeld +=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden +=
            InventoryUIHeldItemController_OnHidden;
    }

    private void OnDestroy()
    {
        inventoryUIHeldItemController.OnItemHeld -=
            InventoryUIHeldItemController_OnItemHeld;
        inventoryUIHeldItemController.OnHidden -=
            InventoryUIHeldItemController_OnHidden;
    }

    protected virtual void DisplayUI()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(uiGameObjects);
        inventoryUIManager.EnableCurrentInventoryUI();
    }

    protected abstract void InventoryUIHeldItemController_OnItemHeld();

    protected abstract void InventoryUIHeldItemController_OnHidden();
}
