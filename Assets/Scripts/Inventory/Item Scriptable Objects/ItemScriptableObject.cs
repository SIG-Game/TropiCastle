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
}
