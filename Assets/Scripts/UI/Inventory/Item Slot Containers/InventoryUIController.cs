using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] protected Inventory inventory;
    [SerializeField] protected List<ItemSlotController> itemSlotControllers;

#if UNITY_EDITOR
    private const string setClickableItemSlotHandlerIndexes =
        "Set Clickable Item Slot Handler Indexes";
#endif

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

    protected virtual void Inventory_OnItemChangedAtIndex(
        ItemStack item, int index)
    {
        itemSlotControllers[index].UpdateUsingItem(item);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        for (int i = 0; i < itemSlotControllers.Count; ++i)
        {
            itemSlotControllers[i].UpdateUsingItem(inventory.GetItemAtIndex(i));

            if (itemSlotControllers[i].TryGetComponent<ClickableItemSlotHandler>(
                out var clickableItemSlotHandler))
            {
                clickableItemSlotHandler.Inventory = inventory;
            }
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

    [ContextMenu(setClickableItemSlotHandlerIndexes)]
    private void SetClickableItemSlotHandlerIndexes()
    {
        ClickableItemSlotHandler[] clickableItemSlotHandlers =
            GetComponentsInChildren<ClickableItemSlotHandler>(true);

        Undo.RecordObjects(clickableItemSlotHandlers,
            setClickableItemSlotHandlerIndexes);

        for (int i = 0; i < clickableItemSlotHandlers.Length; ++i)
        {
            clickableItemSlotHandlers[i].SlotItemIndex = i;
        }
    }
#endif
}
