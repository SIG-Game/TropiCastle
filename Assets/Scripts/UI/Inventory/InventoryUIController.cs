using System;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private Color highlightedSlotColor;

    private Color unhighlightedSlotColor;

    public event Action OnInventoryClosed = delegate { };

    public static bool InventoryUIOpen;

    static InventoryUIController()
    {
        InventoryUIOpen = false;
    }

    private void Awake()
    {
        unhighlightedSlotColor = ItemSlotContainerHelper.GetUnhighlightedSlotColor(inventoryItemSlotContainer);

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
        HighlightInventoryItemSlotAtIndex(index);
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightInventoryItemSlotAtIndex(index);
    }

    public void SetInventorySpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        ItemSlotContainerHelper.SetItemSlotSpriteAtIndex(inventoryItemSlotContainer, slotIndex, sprite);
    }

    private void HighlightInventoryItemSlotAtIndex(int index)
    {
        ItemSlotContainerHelper.SetItemSlotColorAtIndex(inventoryItemSlotContainer, index, highlightedSlotColor);
    }

    private void UnhighlightInventoryItemSlotAtIndex(int index)
    {
        ItemSlotContainerHelper.SetItemSlotColorAtIndex(inventoryItemSlotContainer, index, unhighlightedSlotColor);
    }

    public Inventory GetInventory() => inventory;
}
