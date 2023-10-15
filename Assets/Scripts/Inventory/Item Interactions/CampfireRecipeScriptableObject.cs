using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/Campfire Recipe")]
public class CampfireRecipeScriptableObject : ScriptableObject
{
    public List<ItemStack> PossibleInputItems;
    public ItemStack ResultItem;
}
