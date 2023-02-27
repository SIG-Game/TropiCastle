using System.Collections.Generic;
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

    public static Color GetUnhighlightedSlotColor(Transform itemSlotContainer) =>
        itemSlotContainer.GetChild(0).GetComponent<Image>().color;

    public static List<ItemSlotController> GetItemSlotControllers(Transform itemSlotContainer)
    {
        List<ItemSlotController> itemSlotControllers = new List<ItemSlotController>();

        foreach (Transform itemSlot in itemSlotContainer)
        {
            itemSlotControllers.Add(itemSlot.GetComponent<ItemSlotController>());
        }

        return itemSlotControllers;
    }
}
