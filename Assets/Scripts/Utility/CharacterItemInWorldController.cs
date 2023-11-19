public class CharacterItemInWorldController : CharacterObjectInWorldController
{
    public void ShowItem(ItemStack item)
    {
        Show(item.itemDefinition.Sprite);
    }
}
