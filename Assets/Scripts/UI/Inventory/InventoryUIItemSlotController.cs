using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Inventory inventory;

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
}
