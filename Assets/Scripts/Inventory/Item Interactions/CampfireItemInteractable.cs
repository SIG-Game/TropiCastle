public class CampfireItemInteractable : ItemInteractable
{
    private CampfireUIController campfireUIController;

    public override void Interact(PlayerController playerController)
    {
        campfireUIController.Show();
    }

    public override void SetUpUsingDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        campfireUIController =
            itemInteractableDependencies.GetCampfireUIController();
    }
}
