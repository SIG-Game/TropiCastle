using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject durabilityMeterBackground;
    [SerializeField] private RectTransform durabilityMeter;

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

    public void SetItemInstanceProperties(object itemInstanceProperties,
        ItemScriptableObject itemScriptableObject)
    {
        if (itemInstanceProperties is BreakableItemInstanceProperties
            breakableItemInstanceProperties)
        {
            var fishingRodProperties =
                itemInstanceProperties as BreakableItemInstanceProperties;

            durabilityMeterBackground.SetActive(true);

            float durabilityMeterXScale = (float)fishingRodProperties.Durability /
                ((BreakableItemScriptableObject)itemScriptableObject).InitialDurability;

            durabilityMeter.localScale = new Vector3(durabilityMeterXScale,
                durabilityMeter.localScale.y, durabilityMeter.localScale.z);
        }
        else if (durabilityMeterBackground.activeSelf)
        {
            durabilityMeterBackground.SetActive(false);
        }
    }

    public void UpdateUsingItem(ItemWithAmount item)
    {
        SetSprite(item.itemData.sprite);
        SetAmountText(item.GetAmountText());
        SetItemInstanceProperties(item.instanceProperties, item.itemData);
    }
}
