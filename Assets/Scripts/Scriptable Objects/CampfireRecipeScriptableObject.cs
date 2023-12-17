using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/Campfire Recipe")]
public class CampfireRecipeScriptableObject : ScriptableObject
{
    public List<ItemStackStruct> PossibleInputItems;
    public ItemStackStruct ResultItem;
    public float CookTime;
}
