public class FishingRodItemUsage : IItemUsage
{
    public void UseItem(PlayerController playerController)
    {
        playerController.Fish();
    }
}
