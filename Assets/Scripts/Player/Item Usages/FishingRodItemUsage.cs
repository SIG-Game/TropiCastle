using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private FishingUIController fishingUIController;

    private PlayerController playerController;

    public void UseItem(PlayerController playerController)
    {
        if (this.playerController == null)
        {
            this.playerController = playerController;
        }

        playerController.Fish();

        fishingUIController.OnFishingStopped += FishingUIController_OnFishingStopped;
    }

    private void DecreaseFishingRodDurability(PlayerController playerController)
    {
        ItemWithAmount fishingRodItem = playerController.GetSelectedItem();

        var fishingRodItemInstanceProperties =
            ((FishingRodItemInstanceProperties)fishingRodItem.instanceProperties);

        fishingRodItemInstanceProperties.Durability--;

        if (fishingRodItemInstanceProperties.Durability == 0)
        {
            Inventory playerInventory = playerController.GetInventory();

            playerInventory.RemoveItemAtIndex(playerController.GetSelectedItemIndex());
        }
    }

    private void FishingUIController_OnFishingStopped()
    {
        DecreaseFishingRodDurability(playerController);

        fishingUIController.OnFishingStopped -= FishingUIController_OnFishingStopped;
    }
}
