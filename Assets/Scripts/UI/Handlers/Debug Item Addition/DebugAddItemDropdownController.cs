using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DebugAddItemDropdownController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

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
            TMP_Dropdown.OptionData itemOption = new TMP_Dropdown.OptionData(itemScriptableObject.Name, itemScriptableObject.Sprite);
            dropdown.options.Add(itemOption);
        }

        ScrollRectVerticalNormalizedPosition = 1f;
    }

    private void Start()
    {
        if (!DebugController.DebugModeEnabled)
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

    public ItemScriptableObject GetSelectedItemScriptableObject() => itemScriptableObjects[dropdown.value];
}
