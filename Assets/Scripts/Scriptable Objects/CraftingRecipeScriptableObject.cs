using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/Crafting Recipe")]
public class CraftingRecipeScriptableObject : ScriptableObject
{
    public List<ItemStackStruct> Ingredients;
    public ItemStackStruct ResultItem;
}
