using UnityEngine;

public class TradeItemTooltipController : MonoBehaviour, IElementWithTooltip
{
    public ItemStackStruct Item { private get; set; }

    public string GetTooltipText() => Item.ItemDefinition.GetTooltipText();
}
