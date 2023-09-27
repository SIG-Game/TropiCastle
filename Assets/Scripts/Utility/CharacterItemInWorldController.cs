public class CharacterItemInWorldController : CharacterObjectInWorldController
{
    public void ShowItem(ItemWithAmount item)
    {
        Show(item.itemDefinition.sprite);
    }
}
