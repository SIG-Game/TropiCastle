using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetInventoryGetter;
    [SerializeField] private Transform hotbarItemSlotContainer;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private GameObject inventoryUI;

    private Inventory inventory;

    public event Action OnInventoryClosed = delegate { };

    private const int hotbarSize = 10;

    private void Start()
    {
        if (targetInventoryGetter is IInventoryGetter inventoryGetter)
        {
            inventory = inventoryGetter.GetInventory();
        }
        else
        {
            Debug.LogError($"{nameof(targetInventoryGetter)} does not implement {nameof(IInventoryGetter)}");
        }

        inventory.ChangedItemAt += Inventory_ChangedItemAt;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory") &&
            (!PauseController.Instance.GamePaused || inventoryUI.activeInHierarchy))
        {
            PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;
            inventoryUI.SetActive(PauseController.Instance.GamePaused);

            if (!inventoryUI.activeInHierarchy)
            {
                UpdateSpritesInAllHotbarSlots();

                OnInventoryClosed();
            }
        }
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAt -= Inventory_ChangedItemAt;

        OnInventoryClosed = delegate { };
    }

    private void Inventory_ChangedItemAt(int index)
    {
        ItemWithAmount item = inventory.GetItemAtIndex(index);

        SetSpriteAtSlotIndex(item.itemData.sprite, index);
    }

    public void SetSpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        if (slotIndex < hotbarSize && !inventoryUI.activeInHierarchy)
        {
            SetSpriteAtSlotIndexInContainer(sprite, slotIndex, hotbarItemSlotContainer);
        }

        SetSpriteAtSlotIndexInContainer(sprite, slotIndex, inventoryItemSlotContainer);
    }

    private void UpdateSpritesInAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            SetSpriteAtSlotIndexInContainer(inventory.GetItemAtIndex(i).itemData.sprite, i, hotbarItemSlotContainer);
        }
    }

    private void SetSpriteAtSlotIndexInContainer(Sprite sprite, int slotIndex, Transform itemSlotContainer)
    {
        itemSlotContainer.GetChild(slotIndex).GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    public Inventory GetInventory() => inventory;
}
