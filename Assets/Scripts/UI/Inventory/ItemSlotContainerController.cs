using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemSlotContainerController : MonoBehaviour
{
    [SerializeField] private Color highlightedSlotColor;
    [SerializeField] private Color unhighlightedSlotColor;
    [SerializeField] private List<ItemSlotController> itemSlotControllers;

    [ContextMenu("Set Item Slot Controllers")]
    private void SetItemSlotControllers()
    {
        itemSlotControllers = new List<ItemSlotController>();

        foreach (Transform itemSlot in transform)
        {
            itemSlotControllers.Add(itemSlot.GetComponent<ItemSlotController>());
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [ContextMenu("Set Unhighlighted Slot Color")]
    private void SetUnhighlightedSlotColor()
    {
        if (itemSlotControllers.Count == 0)
        {
            Debug.LogWarning($"No {nameof(itemSlotControllers)} set, so " +
                $"{nameof(unhighlightedSlotColor)} will not be set.");
        }
        else
        {
            unhighlightedSlotColor = itemSlotControllers[0].GetComponent<Image>().color;

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
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
