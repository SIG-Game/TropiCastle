using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private FishingUIController fishingUIController;

    public void UseItem(ItemStack _, int _1)
    {
        if (playerController.WaterInteractionCast(0.5f, 0.4f).collider == null)
        {
            return;
        }

        fishingUIController.OnFishingStopped += FishingUIController_OnFishingStopped;

        fishingUIController.StartFishing();
    }

    private void FishingUIController_OnFishingStopped()
    {
        playerInventory.DecrementItemDurabilityAtIndex(
            playerController.GetSelectedItemIndex());

        fishingUIController.OnFishingStopped -= FishingUIController_OnFishingStopped;
    }
}
