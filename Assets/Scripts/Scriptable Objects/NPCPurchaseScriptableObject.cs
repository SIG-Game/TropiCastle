using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NPC Purchase")]
public class NPCPurchaseScriptableObject : ScriptableObject
{
    public ItemScriptableObject ItemDefinition;
    public int Payment;
}
