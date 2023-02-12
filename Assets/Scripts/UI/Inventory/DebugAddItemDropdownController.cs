using TMPro;
using UnityEngine;

public class DebugAddItemDropdownController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private ItemScriptableObject[] itemScriptableObjects;

    private void Awake()
    {
        itemScriptableObjects = Resources.LoadAll<ItemScriptableObject>("Items");

        foreach (ItemScriptableObject itemScriptableObject in itemScriptableObjects)
        {
            TMP_Dropdown.OptionData itemOption = new TMP_Dropdown.OptionData(itemScriptableObject.name, itemScriptableObject.sprite);
            dropdown.options.Add(itemOption);
        }
    }

    public ItemScriptableObject GetSelectedItemScriptableObject() => itemScriptableObjects[dropdown.value];
}
