using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;

    [Inject] private FishingUIController fishingUIController;

    private int itemIndex;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void UseItem(int itemIndex)
    {
        if (playerController.InWater ||
            playerController.WaterInteractionCast(0.5f, 0.4f).collider == null)
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
