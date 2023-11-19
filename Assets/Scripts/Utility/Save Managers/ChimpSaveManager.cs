using System;
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

        var saveManagerState = new ChimpSaveManagerState
        {
            SaveGuid = saveGuid,
            TimeUntilNextGive = timeUntilNextGive
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        ChimpSaveManagerState chimpState = (ChimpSaveManagerState)saveManagerState;

        chimp.TimeBetweenGives = chimpState.TimeUntilNextGive;

        chimp.LastGiveTime = Time.time;
    }

    [Serializable]
    public class ChimpSaveManagerState : SaveManagerState
    {
        public float TimeUntilNextGive;
    }
}
