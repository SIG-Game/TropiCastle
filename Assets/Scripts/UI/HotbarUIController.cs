using UnityEngine;
using UnityEngine.UI;

public class HotbarUIController : MonoBehaviour
{
    [SerializeField] private Transform hotbarItemSlotContainer;
    [SerializeField] private Transform inventoryItemSlotContainer;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private Color highlightedSlotColor;

    private Inventory inventory;
    private Color unhighlightedSlotColor;
    private int hotbarSize;

    public int SelectedItemIndex { get; private set; }

    private void Awake()
    {
        SelectedItemIndex = 0;

        unhighlightedSlotColor = hotbarItemSlotContainer.GetChild(0).GetComponent<Image>().color;
        hotbarSize = hotbarItemSlotContainer.childCount;

        HighlightItemSlotAtIndex(hotbarItemSlotContainer, SelectedItemIndex);
        HighlightItemSlotAtIndex(inventoryItemSlotContainer, SelectedItemIndex);

        inventoryUIController.OnInventoryClosed += InventoryUIController_OnInventoryClosed;
    }

    private void Start()
    {
        // Runs after InventoryUIController Start method due to execution
        // order because that's where inventoryUIController's inventory is set
        inventory = inventoryUIController.GetInventory();
        inventory.ChangedItemAtIndex += Inventory_ChangedItemAtIndex;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            int newSelectedItemIndex = SelectedItemIndex - (int)Mathf.Sign(Input.mouseScrollDelta.y);

            if (newSelectedItemIndex == hotbarSize)
                newSelectedItemIndex = 0;
            else if (newSelectedItemIndex == -1)
                newSelectedItemIndex = hotbarSize - 1;

            SelectHotbarItem(newSelectedItemIndex);
        }

        ProcessNumberKeysForItemSelection();
    }

    private void OnDestroy()
    {
        inventory.ChangedItemAtIndex -= Inventory_ChangedItemAtIndex;
        inventoryUIController.OnInventoryClosed -= InventoryUIController_OnInventoryClosed;
    }

    private void Inventory_ChangedItemAtIndex(ItemWithAmount item, int index)
    {
        if (index >= hotbarSize || inventoryUIController.IsInventoryUIOpen())
        {
            return;
        }

        Sprite changedItemSprite = item.itemData.sprite;

        InventoryUIController.SetSpriteAtSlotIndexInContainer(changedItemSprite, index, hotbarItemSlotContainer);
    }

    private void InventoryUIController_OnInventoryClosed()
    {
        UpdateSpritesInAllHotbarSlots();
    }

    private void UpdateSpritesInAllHotbarSlots()
    {
        for (int i = 0; i < hotbarSize; ++i)
        {
            Sprite itemSpriteAtCurrentIndex = inventory.GetItemAtIndex(i).itemData.sprite;

            InventoryUIController.SetSpriteAtSlotIndexInContainer(itemSpriteAtCurrentIndex, i,
                hotbarItemSlotContainer);
        }
    }

    private void SelectHotbarItem(int newSelectedItemIndex)
    {
        UnhighlightItemSlotAtIndex(hotbarItemSlotContainer, SelectedItemIndex);
        UnhighlightItemSlotAtIndex(inventoryItemSlotContainer, SelectedItemIndex);

        SelectedItemIndex = newSelectedItemIndex;

        HighlightItemSlotAtIndex(hotbarItemSlotContainer, SelectedItemIndex);
        HighlightItemSlotAtIndex(inventoryItemSlotContainer, SelectedItemIndex);
    }

    private void UnhighlightItemSlotAtIndex(Transform itemSlotContainer, int index)
    {
        SetItemSlotColorAtIndex(itemSlotContainer, index, unhighlightedSlotColor);
    }

    private void HighlightItemSlotAtIndex(Transform itemSlotContainer, int index)
    {
        SetItemSlotColorAtIndex(itemSlotContainer, index, highlightedSlotColor);
    }

    private static void SetItemSlotColorAtIndex(Transform itemSlotContainer, int index, Color color)
    {
        itemSlotContainer.GetChild(index).GetComponent<Image>().color = color;
    }

    private void ProcessNumberKeysForItemSelection()
    {
        int newSelectedItemIndex = SelectedItemIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            newSelectedItemIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            newSelectedItemIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            newSelectedItemIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            newSelectedItemIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            newSelectedItemIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            newSelectedItemIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            newSelectedItemIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            newSelectedItemIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            newSelectedItemIndex = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            newSelectedItemIndex = 9;

        if (newSelectedItemIndex != SelectedItemIndex)
            SelectHotbarItem(newSelectedItemIndex);
    }
}
