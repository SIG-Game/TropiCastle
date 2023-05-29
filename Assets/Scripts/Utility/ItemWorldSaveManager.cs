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
            ItemScriptableObject itemScriptableObject =
                Resources.Load<ItemScriptableObject>(
                    $"Items/{itemWorldState.Item.ItemName}");

            ItemWithAmount item = new ItemWithAmount(itemScriptableObject,
                itemWorldState.Item.Amount,
                itemWorldState.Item.InstanceProperties);

            ItemWorld spawnedItemWorld = ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                itemWorldState.Position, item);

            spawnedItemWorld.gameObject.name = itemWorldState.GameObjectName;
        }
    }
}
