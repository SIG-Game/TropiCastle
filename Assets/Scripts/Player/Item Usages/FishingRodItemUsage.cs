using System;
using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private FishingUIController fishingUIController;

    public event Action OnFishingRodUsed = delegate { };

    private LayerMask waterMask;
    private int fishingRodItemIndex;

    private void Awake()
    {
        waterMask = LayerMask.GetMask("Water");
    }

    private void OnDestroy()
    {
        OnFishingRodUsed = delegate { };
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

        OnFishingRodUsed();
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
