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

    private void ItemSelectionController_OnItemSelectedAtIndex(int index)
    {
        itemSlotControllers[index].Highlight();
    }

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index)
    {
        itemSlotControllers[index].Unhighlight();
    }
}
