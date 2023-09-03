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
    [SerializeField] private Color highlightedBackgroundColor;
    [SerializeField] private Color unhighlightedBackgroundColor;

    private bool disableUpdatingUsingInventory = false;

    public void UpdateUsingItem(ItemWithAmount item,
        bool updateUsingInventory)
    {
        if (updateUsingInventory && disableUpdatingUsingInventory)
        {
            return;
        }

        itemSlotImage.sprite = item.itemDefinition.sprite;
        amountText.text = item.GetAmountText();
        durabilityMeter.UpdateUsingItem(item);
    }

    public void Clear()
    {
        itemSlotImage.sprite = transparentSprite;
        amountText.text = string.Empty;
        durabilityMeter.HideMeter();
    }

    public void Highlight()
    {
        itemSlotBackgroundImage.color = highlightedBackgroundColor;
    }

    public void Unhighlight()
    {
        itemSlotBackgroundImage.color = unhighlightedBackgroundColor;
    }

    public void EnableUpdatingUsingInventory()
    {
        disableUpdatingUsingInventory = false;
    }

    public void DisableUpdatingUsingInventory()
    {
        disableUpdatingUsingInventory = true;
    }
}
