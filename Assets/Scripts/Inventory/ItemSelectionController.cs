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

    public event Action<int> OnItemSelectedAtIndex = delegate { };
    public event Action<int> OnItemDeselectedAtIndex = delegate { };

    private void Start()
    {
        selectedItemIndex = 0;
        OnItemSelectedAtIndex(selectedItemIndex);
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            int newSelectedItemIndex = SelectedItemIndex - (int)Mathf.Sign(Input.mouseScrollDelta.y);

            if (newSelectedItemIndex == onePlusMaxSelectedItemIndex)
                newSelectedItemIndex = 0;
            else if (newSelectedItemIndex == -1)
                newSelectedItemIndex = onePlusMaxSelectedItemIndex - 1;

            SelectedItemIndex = newSelectedItemIndex;
        }

        ProcessNumberKeysToUpdateSelectedItemIndex();
    }

    private void ProcessNumberKeysToUpdateSelectedItemIndex()
    {
        int newSelectedItemIndex = SelectedItemIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            newSelectedItemIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            newSelectedItemIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            newSelectedItemIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            newSelectedItemIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            newSelectedItemIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            newSelectedItemIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            newSelectedItemIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            newSelectedItemIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            newSelectedItemIndex = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            newSelectedItemIndex = 9;

        SelectedItemIndex = newSelectedItemIndex;
    }
}
