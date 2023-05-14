using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;
    [SerializeField] private TextMeshProUGUI amountText;

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
        if (amount == 0)
        {
            amountText.text = string.Empty;
        }
        else
        {
            amountText.text = amount.ToString();
        }
    }
}
