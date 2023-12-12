using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

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

    protected virtual void Inventory_OnItemChangedAtIndex(ItemStack item, int index)
    {
        UpdateSlotAtIndexUsingItem(index, item);
    }

    protected void UpdateSlotAtIndexUsingItem(int slotIndex, ItemStack item)
    {
        itemSlotControllers[slotIndex].UpdateUsingItem(item);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        List<ItemStack> itemList = inventory.GetItemList();

        for (int i = 0; i < itemSlotControllers.Count; ++i)
        {
            InventoryUIItemSlotController itemSlotController =
                itemSlotControllers[i] as InventoryUIItemSlotController;
            ItemStack item = itemList[i];

            itemSlotController.Inventory = inventory;

            itemSlotController.UpdateUsingItem(item);
        }

        inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
    }

    public void UnsetInventory()
    {
        inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;

        inventory = null;
    }

#if UNITY_EDITOR
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
            inventoryUIItemSlot.SlotItemIndex = currentSlotItemIndex;

            ++currentSlotItemIndex;
        }
    }
#endif
}
