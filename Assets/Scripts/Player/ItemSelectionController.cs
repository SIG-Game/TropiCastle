using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSelectionController : MonoBehaviour
{
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InputActionReference selectLeftItemActionReference;
    [SerializeField] private InputActionReference selectRightItemActionReference;
    [SerializeField] private int maxSelectedItemIndex;
    [SerializeField] private float selectInputRepeatTimeSeconds;

    private Coroutine repeatSelectLeftCoroutine;
    private Coroutine repeatSelectRightCoroutine;
    private WaitForSeconds selectInputRepeatWaitForSeconds;
    private InputAction selectLeftItemAction;
    private InputAction selectRightItemAction;
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

    public event Action<int> OnItemSelectedAtIndex = (_) => {};
    public event Action<int> OnItemDeselectedAtIndex = (_) => {};

    private void Awake()
    {
        selectInputRepeatWaitForSeconds = new WaitForSeconds(selectInputRepeatTimeSeconds);

        selectLeftItemAction = selectLeftItemActionReference.action;
        selectRightItemAction = selectRightItemActionReference.action;

        CanScroll = true;
        CanSelect = true;
    }

    // Runs after SaveController loads saved game data, which can change selectedItemIndex
    private void Start()
    {
        OnItemSelectedAtIndex(selectedItemIndex);
    }

    private void Update()
    {
        StopRepeatSelectCoroutines();

        if (!CanSelect ||
            (PauseController.Instance.GamePaused &&
                !inventoryUIManager.InventoryUIOpen))
        {
            return;
        }

        if (CanScroll && Input.mouseScrollDelta.y != 0f)
        {
            int newSelectedItemIndex = 
                SelectedItemIndex - Math.Sign(Input.mouseScrollDelta.y);

            SelectedItemIndex = ClampSelectedItemIndex(newSelectedItemIndex);
        }

        int? numberKeyIndex = inputManager.GetNumberKeyIndexIfUnusedThisFrame();
        if (numberKeyIndex.HasValue)
        {
            SelectedItemIndex = numberKeyIndex.Value;
        }

        if (selectLeftItemAction.IsPressed() && 
            repeatSelectLeftCoroutine == null)
        {
            repeatSelectLeftCoroutine =
                StartCoroutine(RepeatSelectLeftCoroutine());
        }

        if (selectRightItemAction.IsPressed() &&
            repeatSelectRightCoroutine == null)
        {
            repeatSelectRightCoroutine =
                StartCoroutine(RepeatSelectRightCoroutine());
        }
    }

    private int ClampSelectedItemIndex(int selectedItemIndex)
    {
        int clampedSelectedItemIndex;

        if (selectedItemIndex > maxSelectedItemIndex)
        {
            clampedSelectedItemIndex = 0;
        }
        else if (selectedItemIndex < 0)
        {
            clampedSelectedItemIndex = maxSelectedItemIndex;
        }
        else
        {
            clampedSelectedItemIndex = selectedItemIndex;
        }

        return clampedSelectedItemIndex;
    }

    private void StopRepeatSelectCoroutines()
    {
        if (!selectLeftItemAction.IsPressed() &&
            repeatSelectLeftCoroutine != null)
        {
            StopCoroutine(repeatSelectLeftCoroutine);

            repeatSelectLeftCoroutine = null;
        }

        if (!selectRightItemAction.IsPressed() &&
            repeatSelectRightCoroutine != null)
        {
            StopCoroutine(repeatSelectRightCoroutine);

            repeatSelectRightCoroutine = null;
        }
    }

    private IEnumerator RepeatSelectLeftCoroutine() =>
        RepeatSelectWithIndexOffsetCoroutine(-1);

    private IEnumerator RepeatSelectRightCoroutine() =>
        RepeatSelectWithIndexOffsetCoroutine(1);

    private IEnumerator RepeatSelectWithIndexOffsetCoroutine(int indexOffset)
    {
        while (true)
        {
            SelectedItemIndex = ClampSelectedItemIndex(SelectedItemIndex + indexOffset);

            yield return selectInputRepeatWaitForSeconds;
        }
    }
}
