public class ChestItemInteractable : ContainerItemInteractable
{
    [Inject] private ChestUIController chestUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        chestUIController.ShowChestUI(inventory);
    }
}
