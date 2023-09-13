using System.Linq;
using UnityEngine;
using static ItemWorld;

public class ItemWorldSaveManager : MonoBehaviour,
    ISaveManager<SavableItemWorldState>
{
    public SavableItemWorldState[] GetStates()
    {
        ItemWorld[] itemWorlds = FindObjectsOfType<ItemWorld>();

        SavableItemWorldState[] states = itemWorlds.Select(
            x => (SavableItemWorldState)x.GetSavableState()).ToArray();

        return states;
    }

    public void CreateObjectsFromStates(SavableItemWorldState[] states)
    {
        foreach (SavableItemWorldState state in states)
        {
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorldFromState(state);
        }
    }
}
