using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting Recipe")]
public class CraftingRecipeScriptableObject : ScriptableObject
{
    public List<ItemWithAmount> ingredients;
    public ItemWithAmount resultItem;
}
