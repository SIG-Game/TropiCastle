using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotContainerController : MonoBehaviour
{
    [SerializeField] private Transform itemSlotContainerTransform;
    [SerializeField] private Color highlightedSlotColor;

    private List<ItemSlotController> itemSlotControllers;
    private Color unhighlightedSlotColor;

    private void Awake()
    {
        itemSlotControllers = new List<ItemSlotController>();

        foreach (Transform itemSlot in itemSlotContainerTransform)
        {
            itemSlotControllers.Add(itemSlot.GetComponent<ItemSlotController>());
        }

        if (itemSlotControllers.Count > 0)
        {
            unhighlightedSlotColor = itemSlotControllers[0].GetComponent<Image>().color;
        }
    }

    public void SetSpriteAtSlotIndex(Sprite sprite, int slotIndex)
    {
        itemSlotControllers[slotIndex].SetSprite(sprite);
    }

    public void HighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].SetBackgroundColor(highlightedSlotColor);
    }

    public void UnhighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].SetBackgroundColor(unhighlightedSlotColor);
    }

    public int GetItemSlotCount() => itemSlotControllers.Count;
}
