using UnityEngine;

public class ItemTooltipController : MonoBehaviour, IElementWithTooltip
{
    public ItemStack Item { private get; set; }

    public string GetTooltipText() => Item.ItemDefinition.GetTooltipText();
}
