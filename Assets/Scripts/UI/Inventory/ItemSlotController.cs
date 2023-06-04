using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private ItemDurabilityMeterController durabilityMeter;

    public void SetSprite(Sprite sprite)
    {
        itemSlotImage.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        itemSlotBackgroundImage.color = color;
    }

    public void SetAmountText(string text)
    {
        amountText.text = text;
    }

    public void HideDurabilityMeter()
    {
        durabilityMeter.HideMeter();
    }

    public void UpdateUsingItem(ItemWithAmount item)
    {
        SetSprite(item.itemData.sprite);
        SetAmountText(item.GetAmountText());
        durabilityMeter.UpdateUsingItem(item);
    }
}
