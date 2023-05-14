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
        Inventory playerInventory = playerController.GetInventory();
        int fishingRodItemIndex = playerController.GetSelectedItemIndex();
        ItemWithAmount fishingRodItem = playerInventory.GetItemAtIndex(fishingRodItemIndex);

        var fishingRodItemInstanceProperties =
            ((FishingRodItemInstanceProperties)fishingRodItem.instanceProperties);

        fishingRodItemInstanceProperties.Durability--;

        if (fishingRodItemInstanceProperties.Durability == 0)
        {
            playerInventory.RemoveItemAtIndex(fishingRodItemIndex);
        }
        else
        {
            // Refresh durability meter
            playerInventory.InvokeOnItemChangedAtIndexEvent(fishingRodItem, fishingRodItemIndex);
        }
    }

    private void FishingUIController_OnFishingStopped()
    {
        DecreaseFishingRodDurability(playerController);

        fishingUIController.OnFishingStopped -= FishingUIController_OnFishingStopped;
    }
}
