using System;
using UnityEngine;

public class ChimpSaveManager : SaveManager
{
    [SerializeField] private Chimp chimp;

    public override SaveManagerState GetState()
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

        var saveManagerState = new ChimpSaveManagerState
        {
            SaveGuid = saveGuid,
            TimeSecondsUntilNextGive = timeUntilNextGiveSeconds
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        ChimpSaveManagerState chimpState = (ChimpSaveManagerState)saveManagerState;

        chimp.TimeBetweenGivesSeconds = chimpState.TimeSecondsUntilNextGive;

        chimp.LastGiveTimeSeconds = Time.time;
    }

    [Serializable]
    public class ChimpSaveManagerState : SaveManagerState
    {
        public float TimeSecondsUntilNextGive;
    }
}
