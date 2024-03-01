using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveManager : SaveManager
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CharacterDirectionController directionController;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private MoneyController playerMoneyController;

    public override Dictionary<string, object> GetProperties()
    {
        var properties = new Dictionary<string, object>
        {
            { "Position", playerController.transform.position.ToArray() },
            { "Direction", (int)directionController.Direction },
            { "Health", playerHealthController.Health },
            { "SelectedItemIndex", itemSelectionController.SelectedItemIndex },
            { "Money", playerMoneyController.Money }
        };

        return properties;
    }

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        playerController.transform.position = Vector3Helper.FromArray(
            (float[])properties["Position"]);
        directionController.Direction = (CharacterDirection)
            Convert.ToInt32(properties["Direction"]);
        playerHealthController.SetInitialHealth(
            Convert.ToInt32(properties["Health"]));
        itemSelectionController.SelectedItemIndex =
            Convert.ToInt32(properties["SelectedItemIndex"]);
        playerMoneyController.Money = Convert.ToInt32(properties["Money"]);
    }
}
