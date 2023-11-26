using System.Collections.Generic;
using UnityEngine;

public class ChimpSaveManager : SaveManager
{
    [SerializeField] private Chimp chimp;

    public override SaveManagerState GetState()
    {
        float timeUntilNextGive;

        if (chimp.ItemGiveAvailable())
        {
            timeUntilNextGive = 0f;
        }
        else
        {
            float nextGiveTime = chimp.LastGiveTime + chimp.TimeBetweenGives;

            timeUntilNextGive = nextGiveTime - Time.time;
        }

        var propertyList = new List<Property>()
        {
            new Property("TimeUntilNextGive", timeUntilNextGive.ToString())
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
        chimp.TimeBetweenGives = saveManagerState.Properties
            .GetFloatProperty("TimeUntilNextGive");
    }
}
