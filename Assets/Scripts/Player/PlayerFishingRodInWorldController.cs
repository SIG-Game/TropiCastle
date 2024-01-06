using UnityEngine;

public class PlayerFishingRodInWorldController : CharacterObjectInWorldController
{
    [SerializeField] private DirectionalSprites fishingRodInWorldSprites;

    [Inject] private FishingUIController fishingUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();

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
