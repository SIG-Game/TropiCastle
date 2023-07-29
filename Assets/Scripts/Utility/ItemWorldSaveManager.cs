using System.Linq;
using UnityEngine;
using static ItemWorld;

public class ItemWorldSaveManager : MonoBehaviour,
    ISaveManager<SerializableItemWorldState>
{
    public SerializableItemWorldState[] GetStates()
    {
        ItemWorld[] itemWorlds = FindObjectsOfType<ItemWorld>();

        SerializableItemWorldState[] states =
            itemWorlds.Select(x => x.GetSerializableState()).ToArray();

        return states;
    }

    public void CreateObjectsFromStates(SerializableItemWorldState[] states)
    {
        foreach (SerializableItemWorldState state in states)
        {
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorldFromState(state);
        }
    }
}
