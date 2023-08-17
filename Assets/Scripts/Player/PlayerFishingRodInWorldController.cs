using System.Collections.Generic;
using UnityEngine;

public class PlayerFishingRodInWorldController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fishingRodInWorld;
    [SerializeField] private CharacterDirectionController playerDirectionController;
    [SerializeField] private FishingUIController fishingUIController;

    // Direction order for these variables is up, down, left, right
    [SerializeField] private List<Sprite> fishingRodInWorldSprites;
    [SerializeField] private List<Vector3> fishingRodInWorldOffsets;

    private void Awake()
    {
        fishingUIController.OnFishingUIOpened += FishingUIController_OnFishingUIOpened;
        fishingUIController.OnFishingUIClosed += FishingUIController_OnFishingUIClosed;
    }

    private void OnDestroy()
    {
        fishingUIController.OnFishingUIOpened -= FishingUIController_OnFishingUIOpened;
        fishingUIController.OnFishingUIClosed -= FishingUIController_OnFishingUIClosed;
    }

    private void FishingUIController_OnFishingUIOpened()
    {
        fishingRodInWorld.sprite =
            fishingRodInWorldSprites[(int)playerDirectionController.Direction];
        fishingRodInWorld.transform.localPosition =
            fishingRodInWorldOffsets[(int)playerDirectionController.Direction];

        if (playerDirectionController.Direction == CharacterDirection.Up)
        {
            fishingRodInWorld.sortingOrder = -1;
        }
        else
        {
            fishingRodInWorld.sortingOrder = 1;
        }
    }

    private void FishingUIController_OnFishingUIClosed()
    {
        fishingRodInWorld.sprite = null;
    }
}
