using System;
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

        var properties = new Dictionary<string, object>
        {
            { "TimeUntilNextGive", timeUntilNextGive }
        };

        var saveManagerState = new SaveManagerState
        {
            SaveGuid = saveGuid,
            Properties = properties
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        chimp.TimeBetweenGives =
            Convert.ToSingle(saveManagerState.Properties["TimeUntilNextGive"]);
    }
}
