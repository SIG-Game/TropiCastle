using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MonoBehaviour targetInventoryGetter;
    [SerializeField] private Sprite transparentSprite;
    [SerializeField] private Transform hotbarItemSlotContainer;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject heldItem;
    [SerializeField] private GameObject canvas;

    private Inventory inventory;
    private GraphicRaycaster graphicRaycaster;
    private RectTransform canvasRectTransform;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private bool holdingItem;
    private int heldItemIndex;

    private const int hotbarSize = 10;

    private void Awake()
    {
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        heldItemRectTransform = heldItem.GetComponent<RectTransform>();
        heldItemImage = heldItem.GetComponent<Image>();

        holdingItem = false;
    }

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

        inventory.ChangedItemAt = Inventory_ChangedItemAt;
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
                ResetHeldItem();
                UpdateSpritesInAllHotbarSlots();
            }
        }

        if (holdingItem)
        {
            UpdateHeldItemPosition();
        }
    }

    private void Inventory_ChangedItemAt(int index)
    {
        ItemWithAmount item = inventory.GetItemAtIndex(index);

        SetSpriteAtSlotIndex(item.itemData.sprite, index);
    }

    private void SetSpriteAtSlotIndex(Sprite sprite, int slotIndex)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        if (results.Count == 0)
        {
            return;
        }

        int clickedItemIndex = results[0].gameObject.transform.GetSiblingIndex();
        ItemScriptableObject clickedItemData = inventory.GetItemAtIndex(clickedItemIndex).itemData;

        if (holdingItem)
        {
            if (clickedItemIndex == heldItemIndex)
            {
                // Put held item back
                SetSpriteAtSlotIndex(clickedItemData.sprite, clickedItemIndex);
            }
            else
            {
                inventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
            }

            heldItemImage.sprite = transparentSprite;
            holdingItem = false;
        }
        else if (clickedItemData.name != "Empty")
        {
            // Hold clicked item
            SetSpriteAtSlotIndex(transparentSprite, clickedItemIndex);

            heldItemIndex = clickedItemIndex;
            heldItemImage.sprite = clickedItemData.sprite;
            holdingItem = true;
        }
    }

    private void ResetHeldItem()
    {
        if (holdingItem)
        {
            ItemWithAmount heldItem = inventory.GetItemAtIndex(heldItemIndex);

            SetSpriteAtSlotIndex(heldItem.itemData.sprite, heldItemIndex);

            heldItemImage.sprite = transparentSprite;

            holdingItem = false;
        }
    }

    private void UpdateHeldItemPosition()
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null,
                out Vector2 heldItemAnchoredPosition))
        {
            float halfCanvasRectTransformWidth = canvasRectTransform.rect.width / 2f;
            float halfCanvasRectTransformHeight = canvasRectTransform.rect.height / 2f;

            heldItemAnchoredPosition.x = Mathf.Clamp(heldItemAnchoredPosition.x, -halfCanvasRectTransformWidth, halfCanvasRectTransformWidth);
            heldItemAnchoredPosition.y = Mathf.Clamp(heldItemAnchoredPosition.y, -halfCanvasRectTransformHeight, halfCanvasRectTransformHeight);
            heldItemRectTransform.anchoredPosition = heldItemAnchoredPosition;
        }
        else
        {
            Debug.LogError($"Failed to get {nameof(heldItemAnchoredPosition)} from {nameof(canvasRectTransform)}");
        }
    }
}
