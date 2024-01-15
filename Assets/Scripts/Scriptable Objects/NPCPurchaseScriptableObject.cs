using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NPC Purchase")]
public class NPCPurchaseScriptableObject : ScriptableObject
{
    public ItemStack Item;
    public int Payment;
}
