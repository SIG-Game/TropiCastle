using System;
using System.Linq;
using UnityEngine;
using static ItemWorld;

public class ItemWorldSaveManager : MonoBehaviour, ISavable
{
    public SavableState GetSavableState()
    {
        ItemWorld[] itemWorlds = FindObjectsOfType<ItemWorld>();

        SavableItemWorldState[] itemWorldStates = itemWorlds.Select(
            x => (SavableItemWorldState)x.GetSavableState()).ToArray();

        var savableState = new SavableItemWorldSaveManagerState
        {
            ItemWorldStates = itemWorldStates
        };

        return savableState;
    }

    public void SetPropertiesFromSavableState(SavableState savableState)
    {
        var itemWorldSaveManagerState =
            (SavableItemWorldSaveManagerState)savableState;

        foreach (var itemWorldState in
            itemWorldSaveManagerState.ItemWorldStates)
        {
            ItemWorldPrefabInstanceFactory.Instance
                .SpawnItemWorldFromState(itemWorldState);
        }
    }

    [Serializable]
    public class SavableItemWorldSaveManagerState : SavableState
    {
        public SavableItemWorldState[] ItemWorldStates;
    }
}
