using UnityEngine;

public class InventoryUIWithSelectionController : InventoryUIController
{
    [SerializeField] protected ItemSelectionController itemSelectionController;

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
        itemSlotControllers[slotIndex].Highlight();
    }

    protected void UnhighlightSlotAtIndex(int slotIndex)
    {
        itemSlotControllers[slotIndex].Unhighlight();
    }
}
