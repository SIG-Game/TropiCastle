using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private Color highlightedSlotColor;

    private Color unhighlightedSlotColor;

    private List<ItemSlotController> itemSlotControllers;

    public event Action OnInventoryClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIController()
    {
        InventoryUIOpen = false;
    }

    private void Awake()
    {
        unhighlightedSlotColor = ItemSlotContainerHelper.GetUnhighlightedSlotColor(inventoryItemSlotContainer);

        itemSlotControllers = ItemSlotContainerHelper.GetItemSlotControllers(inventoryItemSlotContainer);

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
        itemSlotControllers[index].SetBackgroundColor(highlightedSlotColor);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        itemSlotControllers[index].SetBackgroundColor(unhighlightedSlotColor);
    }

    public void SetInventorySpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        itemSlotControllers[slotIndex].SetSprite(sprite);
    }

    public Inventory GetInventory() => inventory;
}
