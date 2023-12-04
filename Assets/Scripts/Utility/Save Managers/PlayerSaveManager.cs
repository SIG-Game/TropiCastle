using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveManager : SaveManager
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private HealthController playerHealthController;

    public override SaveManagerState GetState()
    {
        var properties = new Dictionary<string, object>
        {
            { "Position", playerController.transform.position.ToArray() },
            { "Direction", (int)playerController.Direction },
            { "Health", playerHealthController.Health },
            { "SelectedItemIndex", itemSelectionController.SelectedItemIndex }
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = properties
        };

        return saveManagerState;
    }

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        playerController.transform.position = Vector3Helper.FromArray(
            (float[])properties["Position"]);
        playerController.Direction = (CharacterDirection)
            Convert.ToInt32(properties["Direction"]);
        playerHealthController.SetInitialHealth(
            Convert.ToInt32(properties["Health"]));
        itemSelectionController.SelectedItemIndex =
            Convert.ToInt32(properties["SelectedItemIndex"]);
    }
}
