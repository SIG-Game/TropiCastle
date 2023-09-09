using UnityEngine;

[CreateAssetMenu(menuName = "NPC Trade", fileName = "New NPC Trade")]
public class NPCTradeScriptableObject : ScriptableObject
{
    public ItemWithAmount InputItem;
    public ItemWithAmount OutputItem;
}
