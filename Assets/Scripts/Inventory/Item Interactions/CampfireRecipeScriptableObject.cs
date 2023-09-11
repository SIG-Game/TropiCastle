using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Campfire Recipe")]
public class CampfireRecipeScriptableObject : ScriptableObject
{
    public List<ItemWithAmount> PossibleInputItems;
    public ItemWithAmount ResultItem;
}
