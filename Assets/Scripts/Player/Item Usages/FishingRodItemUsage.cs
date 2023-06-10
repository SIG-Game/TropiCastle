using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private FishingUIController fishingUIController;

    private int fishingRodItemIndex;

    public void UseItem(ItemWithAmount _, int itemIndex)
    {
        fishingRodItemIndex = itemIndex;

        playerController.Fish();

        fishingUIController.OnFishingStopped += FishingUIController_OnFishingStopped;
    }

    private void DecreaseFishingRodDurability()
    {
        ItemWithAmount fishingRodItem = playerInventory.GetItemAtIndex(fishingRodItemIndex);

        var fishingRodItemInstanceProperties =
            ((BreakableItemInstanceProperties)fishingRodItem.instanceProperties);

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
        DecreaseFishingRodDurability();

        fishingUIController.OnFishingStopped -= FishingUIController_OnFishingStopped;
    }
}
