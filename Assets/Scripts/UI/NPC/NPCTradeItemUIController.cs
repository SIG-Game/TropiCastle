using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCTradeItemUIController : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemAmountText;

    public void SetUp(ItemStack item)
    {
        itemImage.sprite = item.ItemDefinition.Sprite;

        itemAmountText.text = item.GetAmountText();

        GetComponent<ItemTooltipController>().Item = item;
    }
}
