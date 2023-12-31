using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private ItemDurabilityMeterController durabilityMeter;
    [SerializeField] private Color highlightedBackgroundColor;
    [SerializeField] private Color unhighlightedBackgroundColor;

    public void UpdateUsingItem(ItemStack item)
    {
        itemSlotImage.sprite = item.ItemDefinition.Sprite;
        amountText.text = item.GetAmountText();
        durabilityMeter.UpdateUsingItem(item);
    }

    public void Highlight()
    {
        itemSlotBackgroundImage.color = highlightedBackgroundColor;
    }

    public void Unhighlight()
    {
        itemSlotBackgroundImage.color = unhighlightedBackgroundColor;
    }
}
