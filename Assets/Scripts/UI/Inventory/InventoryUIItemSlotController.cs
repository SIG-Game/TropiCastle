using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIItemSlotController : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;

    private int slotItemIndex;

    private void Awake()
    {
        slotItemIndex = transform.GetSiblingIndex();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIHeldItemController.Instance.HoldingItem())
        {
            return;
        }

        ItemScriptableObject slotItemData = inventory.GetItemAtIndex(slotItemIndex).itemData;

        if (slotItemData.name != "Empty")
        {
            string tooltipText = slotItemData switch
            {
                HealingItemScriptableObject healingItem =>
                    $"{slotItemData.name}\nHeals {healingItem.healAmount} Health",
                WeaponItemScriptableObject weaponItem =>
                    $"{slotItemData.name}\nDeals {weaponItem.damage} Damage",
                _ => slotItemData.name
            };

            InventoryTooltipController.Instance.ShowTooltipWithText(tooltipText);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryUIHeldItemController.Instance.ClickedItemAtIndex(slotItemIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!InventoryUIHeldItemController.Instance.HoldingItem())
        {
            InventoryTooltipController.Instance.ShowTooltipWithText(string.Empty);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        itemSlotImage.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        itemSlotBackgroundImage.color = color;
    }
}
