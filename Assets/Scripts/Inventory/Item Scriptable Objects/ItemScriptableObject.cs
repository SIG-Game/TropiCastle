using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int stackSize = defaultStackSize;
    public bool lockPlacementToGrid;
    public bool oneAtATimePlacement;
    public bool hasCustomColliderSize;
    public bool triggerCollisionPickup;
    public Vector2 customColliderSize;

    private const int defaultStackSize = 99;

    public bool IsEmpty() => name == "Empty";

    public string GetTooltipText() => IsEmpty() ?
        string.Empty : string.Concat(name, GetAdditionalInfo());

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
