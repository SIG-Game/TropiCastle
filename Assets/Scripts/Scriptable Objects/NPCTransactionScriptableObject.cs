using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NPC Transaction")]
public class NPCTransactionScriptableObject : ScriptableObject
{
    public NPCTransactionType TransactionType;
    public ItemStack Item;
    public int Money;
}
