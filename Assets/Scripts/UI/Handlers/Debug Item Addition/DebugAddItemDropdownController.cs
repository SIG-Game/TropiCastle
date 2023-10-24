using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DebugAddItemDropdownController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DebugAddItemAmountInputField debugAddItemAmountInputField;

    private List<ItemScriptableObject> itemScriptableObjects;
    private AsyncOperationHandle<IList<ItemScriptableObject>> itemsLoadHandle;

    public float ScrollRectVerticalNormalizedPosition { get; set; }

    private void Awake()
    {
        itemsLoadHandle = Addressables
            .LoadAssetsAsync<ItemScriptableObject>("item", null);

        itemScriptableObjects = new List<ItemScriptableObject>(
            itemsLoadHandle.WaitForCompletion());

        itemScriptableObjects.RemoveAll(x => x.IsEmpty());

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

    private void OnDestroy()
    {
        if (itemsLoadHandle.IsValid())
        {
            Addressables.Release(itemsLoadHandle);
        }
    }

    public void AddItemDropdown_OnValueChanged(int _)
    {
        debugAddItemAmountInputField.ClampAmountToStackSize();
    }

    public ItemScriptableObject GetSelectedItemScriptableObject() => itemScriptableObjects[dropdown.value];
}
