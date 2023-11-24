using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveManager : SaveManager
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private HealthController playerHealthController;

    public override SaveManagerState GetState()
    {
        var propertyList = new List<Property>
        {
            new Property("Position", playerController.transform.position.ToString()),
            new Property("Direction", ((int)playerController.Direction).ToString()),
            new Property("Health", playerHealthController.CurrentHealth.ToString()),
            new Property("SelectedItemIndex",
                itemSelectionController.SelectedItemIndex.ToString())
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = new PropertyCollection(propertyList)
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        playerController.transform.position =
            saveManagerState.Properties.GetVector3Property("Position");
        playerController.Direction =
            (CharacterDirection)saveManagerState.Properties.GetIntProperty("Direction");
        playerHealthController.CurrentHealth =
            saveManagerState.Properties.GetIntProperty("Health");
        itemSelectionController.SelectedItemIndex =
            saveManagerState.Properties.GetIntProperty("SelectedItemIndex");
    }
}
