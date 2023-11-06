using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Recipe/Crafting Recipe")]
public class CraftingRecipeScriptableObject : ScriptableObject
{
    public List<ItemStack> ingredients;
    public ItemStack resultItem;
}
