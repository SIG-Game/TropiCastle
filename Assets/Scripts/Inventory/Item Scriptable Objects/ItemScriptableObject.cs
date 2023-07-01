using UnityEngine;

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
}
