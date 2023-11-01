public class ChestItemInteractable :
    ContainerItemInteractable<ChestItemInstanceProperties>
{
    private ChestUIController chestUIController;

    public override void Interact(PlayerController _)
    {
        chestUIController.SetChestInventory(inventory);

        chestUIController.ShowChestUI();
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        chestUIController =
            itemInteractableDependencies.GetChestUIController();
    }
}
