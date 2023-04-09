using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemScriptableObject : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public bool hasCustomColliderSize;
    public Vector2 customColliderSize;
}
