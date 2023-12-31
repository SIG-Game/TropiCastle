using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private FishingUIController fishingUIController;

    private int itemIndex;

    public void UseItem(ItemStack _, int itemIndex)
    {
        if (playerController.WaterInteractionCast(0.5f, 0.4f).collider == null)
        {
            return;
        }

        this.itemIndex = itemIndex;

        fishingUIController.OnFishingStopped += FishingUIController_OnFishingStopped;

        fishingUIController.StartFishing();
    }

    private void FishingUIController_OnFishingStopped()
    {
        playerInventory.DecrementItemDurabilityAtIndex(itemIndex);

        fishingUIController.OnFishingStopped -= FishingUIController_OnFishingStopped;
    }
}
