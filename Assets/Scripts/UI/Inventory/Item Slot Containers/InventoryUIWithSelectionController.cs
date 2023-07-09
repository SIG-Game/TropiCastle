using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUIWithSelectionController : InventoryUIController
{
    [SerializeField] protected ItemSelectionController itemSelectionController;
    [SerializeField] private Color highlightedSlotColor;
    [SerializeField] private Color unhighlightedSlotColor;

    protected override void Awake()
    {
        base.Awake();

        itemSelectionController.OnItemSelectedAtIndex +=
            ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex +=
            ItemSelectionController_OnItemDeselectedAtIndex;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        itemSelectionController.OnItemSelectedAtIndex -=
            ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -=
            ItemSelectionController_OnItemDeselectedAtIndex;
    }

    protected virtual void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        HighlightSlotAtIndex(index);
    }

    protected virtual void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        UnhighlightSlotAtIndex(index);
    }

    protected void HighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].SetBackgroundColor(highlightedSlotColor);
    }

    protected void UnhighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].SetBackgroundColor(unhighlightedSlotColor);
    }

    [ContextMenu("Set Unhighlighted Slot Color")]
    private void SetUnhighlightedSlotColor()
    {
        if (itemSlotControllers.Count == 0)
        {
            Debug.LogWarning($"No {nameof(itemSlotControllers)} set, so " +
                $"{nameof(unhighlightedSlotColor)} will not be set");
        }
        else
        {
            unhighlightedSlotColor = itemSlotControllers[0].GetComponent<Image>().color;

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
