using System.Collections.Generic;
using UnityEngine;

public class PlayerFishingRodInWorldController : CharacterObjectInWorldController
{
    [SerializeField] private FishingUIController fishingUIController;

    // Direction order for this variable is up, down, left, right
    [SerializeField] private List<Sprite> fishingRodInWorldSprites;

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
        Show(fishingRodInWorldSprites[(int)characterDirectionController.Direction]);
    }
}
