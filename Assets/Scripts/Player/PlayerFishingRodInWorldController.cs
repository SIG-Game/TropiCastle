using UnityEngine;

public class PlayerFishingRodInWorldController : CharacterObjectInWorldController
{
    [SerializeField] private FishingUIController fishingUIController;
    [SerializeField] private DirectionalSprites fishingRodInWorldSprites;

    protected override void Awake()
    {
        base.Awake();

        fishingUIController.OnFishingUIOpened += FishingUIController_OnFishingUIOpened;
        fishingUIController.OnFishingUIClosed += Hide;
    }

    private void OnDestroy()
    {
        fishingUIController.OnFishingUIOpened -= FishingUIController_OnFishingUIOpened;
        fishingUIController.OnFishingUIClosed -= Hide;
    }

    private void FishingUIController_OnFishingUIOpened()
    {
        Show(fishingRodInWorldSprites.ForDirection(
            characterDirectionController.Direction));
    }
}
