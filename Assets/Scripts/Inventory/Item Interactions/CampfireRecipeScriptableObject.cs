using UnityEngine;

[CreateAssetMenu(menuName = "Campfire Recipe")]
public class CampfireRecipeScriptableObject : ScriptableObject
{
    public ItemScriptableObject inputItem;
    public ItemWithAmount resultItem;
}
