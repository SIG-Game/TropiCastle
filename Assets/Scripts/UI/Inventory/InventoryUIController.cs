using System;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private ItemSlotContainerController itemSlotContainer;
    [SerializeField] private ItemSelectionController itemSelectionController;

    public event Action OnInventoryClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIController()
    {
        InventoryUIOpen = false;
    }

    private void Awake()
    {
        inventory.ChangedItemAtIndex += Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex += ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex += ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory") &&
            (!PauseController.Instance.GamePaused || InventoryUIOpen))
        {
            InventoryUIOpen = !InventoryUIOpen;
            PauseController.Instance.GamePaused = InventoryUIOpen;
            inventoryUI.SetActive(InventoryUIOpen);

            if (!InventoryUIOpen)
            {
                OnInventoryClosed();
            }
        }
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
        itemSelectionController.OnItemSelectedAtIndex -= ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -= ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void Inventory_ChangedItemAtIndex(ItemWithAmount item, int index)
    {
        Sprite changedItemSprite = item.itemData.sprite;

        SetInventorySpriteAtSlotIndex(changedItemSprite, index);
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        itemSlotContainer.HighlightSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        itemSlotContainer.UnhighlightSlotAtIndex(index);
    }

    public void SetInventorySpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        itemSlotContainer.SetSpriteAtSlotIndex(sprite, slotIndex);
    }

    public Inventory GetInventory() => inventory;
}
