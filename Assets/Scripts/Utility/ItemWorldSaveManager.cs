using System;
using System.Linq;
using static ItemWorld;

public class ItemWorldSaveManager : SaveManager
{
    public override SavableState GetSavableState()
    {
        ItemWorld[] itemWorlds = FindObjectsOfType<ItemWorld>();

        SavableItemWorldState[] itemWorldStates = itemWorlds.Select(
            x => (SavableItemWorldState)x.GetSavableState()).ToArray();

        var savableState = new SavableItemWorldSaveManagerState
        {
            SaveGuid = saveGuid,
            ItemWorldStates = itemWorldStates
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
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

        public override Type GetSavableClassType() =>
            typeof(ItemWorldSaveManager);
    }
}
