using System;
using UnityEngine;

public class ChimpSaveManager : SaveManager
{
    [SerializeField] private Chimp chimp;

    public override SavableState GetSavableState()
    {
        float timeUntilNextGiveSeconds;

        if (chimp.ItemGiveAvailable())
        {
            timeUntilNextGiveSeconds = 0f;
        }
        else
        {
            float nextGiveTimeSeconds =
                chimp.LastGiveTimeSeconds + chimp.TimeBetweenGivesSeconds;

            timeUntilNextGiveSeconds = nextGiveTimeSeconds - Time.time;
        }

        var savableState = new SavableChimpState
        {
            SaveGuid = saveGuid,
            TimeSecondsUntilNextGive = timeUntilNextGiveSeconds
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
    {
        SavableChimpState chimpState = (SavableChimpState)savableState;

        chimp.TimeBetweenGivesSeconds = chimpState.TimeSecondsUntilNextGive;

        chimp.LastGiveTimeSeconds = Time.time;
    }

    [Serializable]
    public class SavableChimpState : SavableState
    {
        public float TimeSecondsUntilNextGive;

        public override Type GetSavableClassType() =>
            typeof(ChimpSaveManager);
    }
}
