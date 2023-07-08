using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemSlotContainerController : MonoBehaviour
{
    [SerializeField] protected Inventory inventory;
    [SerializeField] protected ItemSelectionController itemSelectionController;
    [SerializeField] private Color highlightedSlotColor;
    [SerializeField] private Color unhighlightedSlotColor;
    [SerializeField] protected List<ItemSlotController> itemSlotControllers;

    protected virtual void Awake()
    {
        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex += Inventory_OnItemChangedAtIndex;
        }

        if (itemSelectionController != null)
        {
            itemSelectionController.OnItemSelectedAtIndex +=
                ItemSelectionController_OnItemSelectedAtIndex;
            itemSelectionController.OnItemDeselectedAtIndex +=
                ItemSelectionController_OnItemDeselectedAtIndex;
        }
    }

    protected virtual void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemChangedAtIndex -= Inventory_OnItemChangedAtIndex;
        }

        if (itemSelectionController != null)
        {
            itemSelectionController.OnItemSelectedAtIndex -=
                ItemSelectionController_OnItemSelectedAtIndex;
            itemSelectionController.OnItemDeselectedAtIndex -=
                ItemSelectionController_OnItemDeselectedAtIndex;
        }
    }

    protected virtual void Inventory_OnItemChangedAtIndex(ItemWithAmount item, int index)
    {
        UpdateSlotAtIndexUsingItem(index, item);
    }

    protected virtual void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        HighlightSlotAtIndex(index);
    }

    protected virtual void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightSlotAtIndex(index);
    }

    public void UpdateSlotAtIndexUsingItem(int slotIndex, ItemWithAmount item)
    {
        itemSlotControllers[slotIndex].UpdateUsingItem(item);
    }

    public void HighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].SetBackgroundColor(highlightedSlotColor);
    }

    public void UnhighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].SetBackgroundColor(unhighlightedSlotColor);
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
            itemSlotController.UpdateUsingItem(item);
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

    [ContextMenu("Set Unhighlighted Slot Color")]
    private void SetUnhighlightedSlotColor()
    {
        if (itemSlotControllers.Count == 0)
        {
            Debug.LogWarning($"No {nameof(itemSlotControllers)} set, so " +
                $"{nameof(unhighlightedSlotColor)} will not be set");
        }
        else
        {
            unhighlightedSlotColor = itemSlotControllers[0].GetComponent<Image>().color;

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
