using UnityEngine;

public class TradeItemTooltipController : MonoBehaviour, IElementWithTooltip
{
    public ItemStack Item { private get; set; }

    public string GetTooltipText() => Item.GetTooltipText();
}
