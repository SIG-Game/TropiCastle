using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIItemSlotController : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUIHeldItemController heldItemController;

    private int slotItemIndex;

    private void Awake()
    {
        slotItemIndex = transform.GetSiblingIndex();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
        heldItemController.ClickedItemAtIndex(slotItemIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltipController.Instance.ShowTooltipWithText(string.Empty);
    }
}
