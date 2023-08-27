using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] protected Inventory inventory;
    [SerializeField] protected List<ItemSlotController> itemSlotControllers;

    protected virtual void Awake()
    {
        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
        }
    }

    protected virtual void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
        }
    }

    protected virtual void Inventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        UpdateSlotAtIndexUsingItem(index, item);
    }

    protected void UpdateSlotAtIndexUsingItem(int slotIndex, ItemWithAmount item)
    {
        itemSlotControllers[slotIndex].UpdateUsingItem(item, true);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        List<ItemWithAmount> itemList = inventory.GetItemList();

        for (int i = 0; i < itemSlotControllers.Count; ++i)
        {
            InventoryUIItemSlotController itemSlotController =
                itemSlotControllers[i] as InventoryUIItemSlotController;
            ItemWithAmount item = itemList[i];

            itemSlotController.SetInventory(inventory);
            itemSlotController.UpdateUsingItem(item, true);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }

    [ContextMenu("Set Item Slot Controllers")]
    private void SetItemSlotControllers()
    {
        ItemSlotController[] childItemSlotControllersArray =
            GetComponentsInChildren<ItemSlotController>(true);

        itemSlotControllers = new List<ItemSlotController>(childItemSlotControllersArray);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [ContextMenu("Set Inventory UI Item Slot Indexes")]
    private void SetInventoryUIItemSlotIndexes()
    {
        InventoryUIItemSlotController[] childInventoryUIItemSlots =
            GetComponentsInChildren<InventoryUIItemSlotController>(true);

        Undo.RecordObjects(childInventoryUIItemSlots, "Set Inventory UI Item Slot Indexes");

        int currentSlotItemIndex = 0;
        foreach (InventoryUIItemSlotController inventoryUIItemSlot in
            childInventoryUIItemSlots)
        {
            inventoryUIItemSlot.SetSlotItemIndex(currentSlotItemIndex);
            ++currentSlotItemIndex;
        }
    }
}
