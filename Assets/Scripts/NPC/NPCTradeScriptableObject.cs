using UnityEngine;

[CreateAssetMenu(menuName = "NPC Trade", fileName = "New NPC Trade")]
public class NPCTradeScriptableObject : ScriptableObject
{
    public ItemScriptableObject InputItem;
    public ItemWithAmount OutputItem;
}
