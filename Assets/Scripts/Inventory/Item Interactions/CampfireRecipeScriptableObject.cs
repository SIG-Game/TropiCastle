using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/Campfire Recipe")]
public class CampfireRecipeScriptableObject : ScriptableObject
{
    public List<ItemWithAmount> PossibleInputItems;
    public ItemWithAmount ResultItem;
}
