using System;
using System.Collections.Generic;
using UnityEngine;

public class ChimpSaveManager : SaveManager
{
    [SerializeField] private Chimp chimp;

    public override Dictionary<string, object> GetProperties()
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

        return properties;
    }

    public override void UpdateFromProperties(Dictionary<string, object> properties)
    {
        chimp.TimeBetweenGives = Convert.ToSingle(properties["TimeUntilNextGive"]);
    }
}
