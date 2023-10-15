using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/NPC Item Offering")]
public class NPCItemOfferingScriptableObject : ScriptableObject
{
    public List<ItemStack> PotentialItemsToGive;
}
