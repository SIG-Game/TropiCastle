using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/NPC Trade")]
public class NPCTradeScriptableObject : ScriptableObject
{
    public List<ItemStack> InputItems;
    public List<ItemStack> OutputItems;
}
