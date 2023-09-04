using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Item")]
public class ItemScriptableObject : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int stackSize = defaultStackSize;
    public bool hasCustomColliderSize;
    public Vector2 customColliderSize;

    private const int defaultStackSize = 99;

    public string GetTooltipText() => name != "Empty" ?
        string.Concat(name, GetAdditionalInfo()) : string.Empty;

    public virtual string GetAdditionalInfo() => string.Empty;

    public static ItemScriptableObject FromName(string name)
    {
        string itemScriptableObjectKey = $"Items/{name}.asset";

        var itemLoadHandle = Addressables
            .LoadAssetAsync<ItemScriptableObject>(itemScriptableObjectKey);

        ItemScriptableObject item = itemLoadHandle.WaitForCompletion();

        if (itemLoadHandle.IsValid())
        {
            Addressables.Release(itemLoadHandle);
        }

        return item;
    }
}
