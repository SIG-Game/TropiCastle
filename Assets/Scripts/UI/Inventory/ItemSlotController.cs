using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private RectTransform durabilityMeter;

    public void SetSprite(Sprite sprite)
    {
        itemSlotImage.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        itemSlotBackgroundImage.color = color;
    }

    public void SetAmountText(int amount)
    {
        if (amount <= 1)
        {
            amountText.text = string.Empty;
        }
        else
        {
            amountText.text = amount.ToString();
        }
    }

    public void SetItemInstanceProperties(object itemInstanceProperties)
    {
        if (itemInstanceProperties is FishingRodItemInstanceProperties)
        {
            var fishingRodProperties =
                itemInstanceProperties as FishingRodItemInstanceProperties;

            float durabilityMeterXScale = (float)fishingRodProperties.Durability /
                FishingRodItemInstanceProperties.InitialDurability;

            durabilityMeter.localScale = new Vector3(durabilityMeterXScale,
                durabilityMeter.localScale.y, durabilityMeter.localScale.z);
        }
        else if (durabilityMeter.localScale.x != 0f)
        {
            durabilityMeter.localScale = new Vector3(0f,
                durabilityMeter.localScale.y, durabilityMeter.localScale.z);
        }
    }

    public void UpdateUsingItem(ItemWithAmount item)
    {
        SetSprite(item.itemData.sprite);
        SetAmountText(item.amount);
        SetItemInstanceProperties(item.instanceProperties);
    }
}
