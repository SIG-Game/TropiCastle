using System.Collections.Generic;
using UnityEngine;

public class ItemSlotSelectionController : MonoBehaviour
{
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private List<ItemSlotController> itemSlotControllers;

    private void Awake()
    {
        itemSelectionController.OnItemSelectedAtIndex +=
            ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex +=
            ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void OnDestroy()
    {
        itemSelectionController.OnItemSelectedAtIndex -=
            ItemSelectionController_OnItemSelectedAtIndex;
        itemSelectionController.OnItemDeselectedAtIndex -=
            ItemSelectionController_OnItemDeselectedAtIndex;
    }

    private void ItemSelectionController_OnItemSelectedAtIndex(int index) =>
        itemSlotControllers[index].Highlight();

    private void ItemSelectionController_OnItemDeselectedAtIndex(int index) =>
        itemSlotControllers[index].Unhighlight();
}
