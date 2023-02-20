using UnityEngine;

public class DebugAddItemDropdownTemplate : MonoBehaviour
{
    [SerializeField] private ItemSelectionController itemSelectionController;

    private void Awake()
    {
        itemSelectionController.CanScroll = false;
    }

    private void OnDestroy()
    {
        itemSelectionController.CanScroll = true;
    }
}
