using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotTooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private IngredientsTooltipController tooltipController;

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
            tooltipController.ShowTooltipWithText(slotItemData.name);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipController.ShowTooltipWithText(string.Empty);
    }
}
