using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Transform inventoryItemSlotContainer;

    public event Action OnInventoryClosed = delegate { };

    private void Awake()
    {
        inventory.ChangedItemAtIndex += Inventory_ChangedItemAtIndex;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory") &&
            (!PauseController.Instance.GamePaused || IsInventoryUIOpen()))
        {
            PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;
            inventoryUI.SetActive(PauseController.Instance.GamePaused);

            if (!IsInventoryUIOpen())
            {
                OnInventoryClosed();
            }
        }
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
    }

    private void Inventory_ChangedItemAtIndex(ItemWithAmount item, int index)
    {
        Sprite changedItemSprite = item.itemData.sprite;

        SetInventorySpriteAtSlotIndex(changedItemSprite, index);
    }

    public void SetInventorySpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        SetSpriteAtSlotIndexInContainer(sprite, slotIndex, inventoryItemSlotContainer);
    }

    public static void SetSpriteAtSlotIndexInContainer(Sprite sprite, int slotIndex, Transform itemSlotContainer)
    {
        itemSlotContainer.GetChild(slotIndex).GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    public Inventory GetInventory() => inventory;

    public bool IsInventoryUIOpen() => inventoryUI.activeInHierarchy;
}
