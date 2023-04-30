using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSelectionController : MonoBehaviour
{
    [SerializeField] private int onePlusMaxSelectedItemIndex;

    private InputAction selectLeftItemAction;
    private InputAction selectRightItemAction;
    private int selectedItemIndex;

    public int SelectedItemIndex
    {
        get => selectedItemIndex;
        private set
        {
            if (selectedItemIndex != value)
            {
                OnItemDeselectedAtIndex(selectedItemIndex);
                selectedItemIndex = value;
                OnItemSelectedAtIndex(selectedItemIndex);
            }
        }
    }

    public bool CanScroll { private get; set; }
    public bool CanSelect { private get; set; }

    public event Action<int> OnItemSelectedAtIndex = delegate { };
    public event Action<int> OnItemDeselectedAtIndex = delegate { };

    private void Start()
    {
        selectLeftItemAction = InputManager.Instance.GetAction("Select Left Item");
        selectRightItemAction = InputManager.Instance.GetAction("Select Right Item");

        selectedItemIndex = 0;
        OnItemSelectedAtIndex(selectedItemIndex);

        CanScroll = true;
        CanSelect = true;
    }

    private void Update()
    {
        if ((PauseController.Instance.GamePaused && !InventoryUIController.InventoryUIOpen) ||
            !CanSelect)
        {
            return;
        }

        if (Input.mouseScrollDelta.y != 0f && CanScroll)
        {
            int newSelectedItemIndex = SelectedItemIndex - (int)Mathf.Sign(Input.mouseScrollDelta.y);

            SelectedItemIndex = ClampSelectedItemIndex(newSelectedItemIndex);
        }

        int numberKeyIndex = InputManager.Instance.GetNumberKeyIndexIfUnusedThisFrame();

        bool numberKeyInputAvailable = numberKeyIndex != -1;
        if (numberKeyInputAvailable)
        {
            SelectedItemIndex = numberKeyIndex;
        }

        if (selectLeftItemAction.WasPerformedThisFrame())
        {
            SelectedItemIndex = ClampSelectedItemIndex(SelectedItemIndex - 1);
        }

        if (selectRightItemAction.WasPerformedThisFrame())
        {
            SelectedItemIndex = ClampSelectedItemIndex(SelectedItemIndex + 1);
        }
    }

    private int ClampSelectedItemIndex(int selectedItemIndex)
    {
        int clampedSelectedItemIndex;

        if (selectedItemIndex >= onePlusMaxSelectedItemIndex)
        {
            clampedSelectedItemIndex = 0;
        }
        else if (selectedItemIndex < 0)
        {
            clampedSelectedItemIndex = onePlusMaxSelectedItemIndex - 1;
        }
        else
        {
            clampedSelectedItemIndex = selectedItemIndex;
        }

        return clampedSelectedItemIndex;
    }
}
