using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Image itemSlotImage;
    [SerializeField] private Image itemSlotBackgroundImage;

    public void SetSprite(Sprite sprite)
    {
        itemSlotImage.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        itemSlotBackgroundImage.color = color;
    }
}
