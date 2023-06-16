using System.Linq;
using UnityEngine;
using static ItemWorld;

public class ItemWorldSaveManager : MonoBehaviour
{
    public SerializableItemWorldState[] GetItemWorldStates()
    {
        ItemWorld[] itemWorlds = FindObjectsOfType<ItemWorld>();

        SerializableItemWorldState[] itemWorldStates =
            itemWorlds.Select(x => x.GetSerializableState()).ToArray();

        return itemWorldStates;
    }

    public void CreateItemWorldsFromStates(SerializableItemWorldState[] itemWorldStates)
    {
        foreach (SerializableItemWorldState itemWorldState in itemWorldStates)
        {
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorldFromState(itemWorldState);
        }
    }
}
