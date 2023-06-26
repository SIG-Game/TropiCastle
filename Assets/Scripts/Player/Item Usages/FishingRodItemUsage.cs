using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private FishingUIController fishingUIController;

    private LayerMask waterMask;
    private int fishingRodItemIndex;

    private void Awake()
    {
        waterMask = LayerMask.GetMask("Water");
    }

    public void UseItem(ItemWithAmount _, int itemIndex)
    {
        if (playerController.InteractionCast(waterMask, 0.5f, 0.4f).collider == null)
        {
            DialogueBox.Instance.PlayDialogue("You must be facing water to fish.");
            return;
        }

        fishingRodItemIndex = itemIndex;

        fishingUIController.OnFishingStopped += FishingUIController_OnFishingStopped;

        fishingUIController.StartFishing();
    }

    private void FishingUIController_OnFishingStopped()
    {
        playerInventory.DecrementItemDurabilityAtIndex(fishingRodItemIndex);

        fishingUIController.OnFishingStopped -= FishingUIController_OnFishingStopped;
    }
}
