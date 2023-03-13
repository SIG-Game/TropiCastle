using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugAddItemDropdownController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private List<ItemScriptableObject> itemScriptableObjects;

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
    }

    public ItemScriptableObject GetSelectedItemScriptableObject() => itemScriptableObjects[dropdown.value];
}
