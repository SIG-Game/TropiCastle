using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour targetInventoryGetter;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private GameObject inventoryUI;

    private Inventory inventory;

    public event Action OnInventoryClosed = delegate { };

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
        inventory.ChangedItemAt -= Inventory_ChangedItemAt;
    }

    private void Inventory_ChangedItemAt(int index)
    {
        Sprite changedItemSprite = inventory.GetItemAtIndex(index).itemData.sprite;

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
