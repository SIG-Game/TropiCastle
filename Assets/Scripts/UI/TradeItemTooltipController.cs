using UnityEngine;

public class TradeItemTooltipController : MonoBehaviour, IElementWithTooltip
{
    public ItemWithAmount Item { private get; set; }

    public string GetTooltipText() => Item.GetTooltipText();
}
