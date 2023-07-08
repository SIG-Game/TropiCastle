using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugAddItemDropdownController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DebugAddItemAmountInputField debugAddItemAmountInputField;

    private List<ItemScriptableObject> itemScriptableObjects;

    public float ScrollRectVerticalNormalizedPosition { get; set; }

    private void Awake()
    {
        itemScriptableObjects = new List<ItemScriptableObject>(
            Resources.LoadAll<ItemScriptableObject>("Items"));

        itemScriptableObjects.RemoveAll(x => x.name == "Empty");

        foreach (ItemScriptableObject itemScriptableObject in itemScriptableObjects)
        {
            TMP_Dropdown.OptionData itemOption = new TMP_Dropdown.OptionData(itemScriptableObject.name, itemScriptableObject.sprite);
            dropdown.options.Add(itemOption);
        }

        ScrollRectVerticalNormalizedPosition = 1f;
    }

    private void Start()
    {
        if (!DebugModeController.DebugModeEnabled)
        {
            dropdown.gameObject.SetActive(false);
        }
    }

    public void AddItemDropdown_OnValueChanged(int _)
    {
        debugAddItemAmountInputField.ClampAmountToStackSize();
    }

    public ItemScriptableObject GetSelectedItemScriptableObject() => itemScriptableObjects[dropdown.value];
}
