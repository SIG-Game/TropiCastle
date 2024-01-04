using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NPC Product")]
public class NPCProductScriptableObject : ScriptableObject
{
    public ItemStack Item;
    public int Cost;
}
