using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/NPC Trade")]
public class NPCTradeScriptableObject : ScriptableObject
{
    public ItemWithAmount InputItem;
    public ItemWithAmount OutputItem;
}
