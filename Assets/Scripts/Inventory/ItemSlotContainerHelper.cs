using UnityEngine;
using UnityEngine.UI;

public static class ItemSlotContainerHelper
{
    public static void SetItemSlotSpriteAtIndex(Transform itemSlotContainer, int index, Sprite sprite)
    {
        itemSlotContainer.GetChild(index).GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    public static void SetItemSlotColorAtIndex(Transform itemSlotContainer, int index, Color color)
    {
        itemSlotContainer.GetChild(index).GetComponent<Image>().color = color;
    }
}
