using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/NPC Trade")]
public class NPCTradeScriptableObject : ScriptableObject
{
    public List<ItemWithAmount> InputItems;
    public ItemWithAmount OutputItem;
}
