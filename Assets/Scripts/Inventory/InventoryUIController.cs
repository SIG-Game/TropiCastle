using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MonoBehaviour targetInventoryGetter;
    [SerializeField] private Sprite transparentSprite;
    [SerializeField] private Transform hotbarItemSlotContainer;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject heldItem;
    [SerializeField] private GameObject canvas;

    private Inventory inventory;
    private GraphicRaycaster graphicRaycaster;
    private RectTransform canvasRectTransform;
    private RectTransform heldItemRectTransform;
    private Image heldItemImage;
    private const int hotbarSize = 10;
    private bool holdingItem = false;
    private int heldItemIndex;

    private void Awake()
    {
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        heldItemRectTransform = heldItem.GetComponent<RectTransform>();
        heldItemImage = heldItem.GetComponent<Image>();
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

    private void Inventory_ChangedItemAt(int index)
    {
        ItemWithAmount item = inventory.GetItemAtIndex(index);

        SetSpriteAtSlotIndex(index, item.itemData.sprite);
    }

    private void SetSpriteAtSlotIndex(int slotIndex, Sprite newSprite)
    {
        if (slotIndex < hotbarSize && !inventoryUI.activeInHierarchy)
        {
            hotbarItemSlotContainer.GetChild(slotIndex).GetChild(0).GetComponent<Image>().sprite = newSprite;
        }

        itemSlotContainer.GetChild(slotIndex).GetChild(0).GetComponent<Image>().sprite = newSprite;
    }

    private void UpdateSpritesInAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            hotbarItemSlotContainer.GetChild(i).GetChild(0).GetComponent<Image>().sprite = inventory.GetItemAtIndex(i).itemData.sprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        if (results.Count > 0)
        {
            int clickedItemIndex = results[0].gameObject.transform.GetSiblingIndex();
            ItemWithAmount clickedItem = inventory.GetItemAtIndex(clickedItemIndex);

            if (holdingItem)
            {
                if (clickedItemIndex == heldItemIndex)
                {
                    // Put held item back
                    SetSpriteAtSlotIndex(clickedItemIndex, clickedItem.itemData.sprite);
                }
                else
                {
                    inventory.SwapItemsAt(heldItemIndex, clickedItemIndex);
                }

                heldItemImage.sprite = transparentSprite;
                holdingItem = false;
            }
            else if (clickedItem.itemData.name != "Empty")
            {
                // Hold clicked item
                SetSpriteAtSlotIndex(clickedItemIndex, transparentSprite);

                heldItemIndex = clickedItemIndex;
                heldItemImage.sprite = clickedItem.itemData.sprite;
                holdingItem = true;
            }
        }
    }

    public void ResetHeldItem()
    {
        if (holdingItem)
        {
            ItemWithAmount heldItem = inventory.GetItemAtIndex(heldItemIndex);

            SetSpriteAtSlotIndex(heldItemIndex, heldItem.itemData.sprite);

            heldItemImage.sprite = transparentSprite;

            holdingItem = false;
        }
    }

    private void Update()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0f, canvasRectTransform.rect.width);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0f, canvasRectTransform.rect.height);
        heldItemRectTransform.anchoredPosition = anchoredPosition;

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
    }
}
