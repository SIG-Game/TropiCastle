using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSelectionController : MonoBehaviour
{
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InputActionReference selectLeftItemActionReference;
    [SerializeField] private InputActionReference selectRightItemActionReference;
    [SerializeField] private int onePlusMaxSelectedItemIndex;
    [SerializeField] private float selectInputRepeatTimeSeconds;

    private InputAction selectLeftItemAction;
    private InputAction selectRightItemAction;
    private Coroutine repeatSelectLeftItemCoroutine;
    private Coroutine repeatSelectRightItemCoroutine;
    private WaitForSeconds selectInputRepeatWaitForSeconds;
    private int selectedItemIndex;

    public int SelectedItemIndex
    {
        get => selectedItemIndex;
        set
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

    private void Awake()
    {
        selectLeftItemAction = selectLeftItemActionReference.action;
        selectRightItemAction = selectRightItemActionReference.action;

        selectedItemIndex = 0;
    }

    // Runs after SaveController loads saved game data, which can change selectedItemIndex
    private void Start()
    {
        selectInputRepeatWaitForSeconds = new WaitForSeconds(selectInputRepeatTimeSeconds);

        OnItemSelectedAtIndex(selectedItemIndex);

        CanScroll = true;
        CanSelect = true;
    }

    private void Update()
    {
        StopRepeatSelectCoroutinesIfNeeded();

        if ((PauseController.Instance.GamePaused &&
            !inventoryUIManager.InventoryUIOpen) ||
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

        if (selectLeftItemAction.IsPressed() && repeatSelectLeftItemCoroutine == null)
        {
            repeatSelectLeftItemCoroutine = StartCoroutine(RepeatSelectLeftItemCoroutine());
        }

        if (selectRightItemAction.IsPressed() && repeatSelectRightItemCoroutine == null)
        {
            repeatSelectRightItemCoroutine = StartCoroutine(RepeatSelectRightItemCoroutine());
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

    private void StopRepeatSelectCoroutinesIfNeeded()
    {
        if (!selectLeftItemAction.IsPressed() && repeatSelectLeftItemCoroutine != null)
        {
            StopCoroutine(repeatSelectLeftItemCoroutine);

            repeatSelectLeftItemCoroutine = null;
        }

        if (!selectRightItemAction.IsPressed() && repeatSelectRightItemCoroutine != null)
        {
            StopCoroutine(repeatSelectRightItemCoroutine);

            repeatSelectRightItemCoroutine = null;
        }
    }

    private IEnumerator RepeatSelectLeftItemCoroutine()
    {
        return RepeatSelectItemWithIndexOffset(-1);
    }

    private IEnumerator RepeatSelectRightItemCoroutine()
    {
        return RepeatSelectItemWithIndexOffset(1);
    }

    private IEnumerator RepeatSelectItemWithIndexOffset(int indexOffset)
    {
        while (true)
        {
            SelectedItemIndex = ClampSelectedItemIndex(SelectedItemIndex + indexOffset);

            yield return selectInputRepeatWaitForSeconds;
        }
    }
}
