using System;
using UnityEngine;

public class ItemSelectionController : MonoBehaviour
{
    [SerializeField] private int onePlusMaxSelectedItemIndex;

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

            if (newSelectedItemIndex == onePlusMaxSelectedItemIndex)
                newSelectedItemIndex = 0;
            else if (newSelectedItemIndex == -1)
                newSelectedItemIndex = onePlusMaxSelectedItemIndex - 1;

            SelectedItemIndex = newSelectedItemIndex;
        }

        int numberKeyIndex = InputManager.Instance.GetNumberKeyIndexIfUnusedThisFrame();

        bool numberKeyInputAvailable = numberKeyIndex != -1;
        if (numberKeyInputAvailable)
        {
            SelectedItemIndex = numberKeyIndex;
        }
    }
}
