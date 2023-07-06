using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private ItemDurabilityMeterController durabilityMeter;
    [SerializeField] private Sprite transparentSprite;

    public virtual void UpdateUsingItem(ItemWithAmount item)
    {
        itemSlotImage.sprite = item.itemData.sprite;
        amountText.text = item.GetAmountText();
        durabilityMeter.UpdateUsingItem(item);
    }

    public void Clear()
    {
        itemSlotImage.sprite = transparentSprite;
        amountText.text = string.Empty;
        durabilityMeter.HideMeter();
    }

    public void SetBackgroundColor(Color color)
    {
        itemSlotBackgroundImage.color = color;
    }
}
